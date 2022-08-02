using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviourPunCallbacks, Photon.Pun.IPunObservable
{
    private GameObject target;

    private NavMeshAgent agent;

    private Animator animator;

    private Collider myCollider;

    [SerializeField] private Health health;

    [HideInInspector] public bool isAttacking = false;

    [HideInInspector] public bool isDead = false;

    public float speed = 1.0f;

    public float angularSpeed = 120;

    public float damage = 20;

    public float sightRange;
    public bool playerInSightRange, playerInAttack;

    private Vector3 syncPos = Vector3.zero;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    private Quaternion syncRot = Quaternion.identity;
    
    public GameObject[] players;

    PhotonView PV;

    public LayerMask whatIsGround;
    
    
    void Start()
    {
        PV = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if(GetClosestTarget() != null)
        {
            float dist = GetDistanceFromTarget(GetClosestTarget());
            if(dist > sightRange)
                playerInSightRange = false;
            else
                playerInSightRange = true;
        }
    }

    void FixedUpdate()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            SyncTransform();
        }
        CheckHealth();
        Patroling();
        Chase();
        CheckAttack();

    }

    float GetDistanceFromTarget(GameObject player)
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    GameObject GetClosestTarget()
    {

        for(int i = 0; i < players.Length; i++)
        {
            if(players[i] == null)
            {
                players = GameObject.FindGameObjectsWithTag("Player");
                return null;
            }
        }
        if(players.Length == 0)
            return null;
        GameObject closestTarget = players[0].gameObject;

        float minDist = 99999999;
        for(int i = 0; i < players.Length; i++)
        {
            float dist = GetDistanceFromTarget(players[i].gameObject);
            Health playerHealth = players[i].GetComponent<Health>();

            if(dist < minDist && playerHealth.value > 0)
            {
                minDist = dist;
                closestTarget = players[i].gameObject;
            }
        }

        return closestTarget;
    }

    void SyncTransform()
    {
        transform.position = Vector3.Lerp(transform.position, syncPos, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, 0.1f);
    }

    void CheckHealth()
    {
        if(isDead) 
            return;
        if(health.value <= 0)
        {
            isDead = true;
            agent.isStopped = true;
            myCollider.enabled = false;

            animator.CrossFadeInFixedTime("Death", 0.1f);
            BroadcastDead();
            DestroyAfterTime(gameObject, 3f);
            
        }
    }

    void DestroyAfterTime(GameObject obj, float time)
    {
        StartCoroutine(CoDestroyAfterTime(obj, time));
    }

    IEnumerator CoDestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if(PV.IsMine)
            PhotonNetwork.Destroy(obj);
    }

    private bool isLateUpdating = false;

    IEnumerator CoLateUpdateDestination(float latency) {
        isLateUpdating = true;

        yield return new WaitForSeconds(latency);

        if(target == null)
        {
            Retargeting();
        }
        else
        {
            agent.destination = target.transform.position;
        }

    }

    private void Patroling()
    {
        if(playerInSightRange){
            walkPointSet = false;
            return;
        } 
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet)
            agent.SetDestination(walkPoint);
        
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        walkPointSet = true;
    }

    void Chase()
    {
        if(!playerInSightRange) return;
        if(isDead) return;
        //the fuck??
        GameObject closestTarget = GetClosestTarget();
        if(closestTarget == null) return;

        Retargeting();

        float dist = GetDistanceFromTarget(target);
        if(dist >= 20)
        {
            if(!isLateUpdating)
            {
                StartCoroutine(CoLateUpdateDestination(0.1f));
            }
        }
        else if(dist <= 40)
        {
            StartCoroutine(CoLateUpdateDestination(0.5f));
        }
        else if(dist <= 60)
        {
            StartCoroutine(CoLateUpdateDestination(1f));
        }
        else
        {
            StartCoroutine(CoLateUpdateDestination(2f));
        }

        
        agent.destination = target.transform.position;
        
        
    }

    GameObject GetCurrentPlayer()
    {
        for(int i= 0; i < players.Length; i++)
        {
            if(players[i].gameObject == target)
            {
                return players[i];
            }
        }
        return null;
    }

    void CheckAttack()
    {
        if(isDead) return;
        if(isAttacking) return;
        
        GameObject closestTarget = GetClosestTarget();
        if(closestTarget == null) return;
        
        if(target == null)
        {
            Retargeting();
            return;
        }
        //check if player still alive? maybe idk add later

        float distanceFromTarget = Vector3.Distance(target.transform.position, transform.position);

        if(distanceFromTarget <= 1.8f)
        {
            Attack();
        }
    }

    void Retargeting()
    {
        GameObject closestTarget = GetClosestTarget();

        if(closestTarget == null) return;

        target = closestTarget;
        
    }

    void Attack()
    {
        Health targetHealth = target.GetComponent<Health>();

        if(targetHealth == null)
        {
            throw new System.Exception("Target doesn't have a Health Component.");
        }

        targetHealth.TakeDamage(damage);

        agent.speed = 0;
        agent.angularSpeed = 0;
        isAttacking = true;
        animator.SetTrigger("ShouldAttack");
        BroadcastAttackAnimation();

        Invoke("ResetAttacking", 1.5f);
    }

    void BroadcastAttackAnimation()
    {
        photonView.RPC("RPCBroadcastAttackAnimation", RpcTarget.Others);
    }

    [PunRPC]
    void RPCBroadcastAttackAnimation()
    {
        animator.SetTrigger("ShouldAttack");
    }

    void BroadcastDead()
    {
        photonView.RPC("RPCBroadcastDead", RpcTarget.Others);
    }

    [PunRPC]
    void RPCBroadcastDead()
    {
        animator.CrossFadeInFixedTime("Death", 0.1f);
        isDead = true;
        agent.isStopped = true;
        myCollider.enabled = false;
        DestroyAfterTime(gameObject, 3f);
    }

    void ResetAttacking()
    {
        isAttacking = false;
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            syncPos = (Vector3) stream.ReceiveNext();
            syncRot = (Quaternion) stream.ReceiveNext();
        }
    }
}