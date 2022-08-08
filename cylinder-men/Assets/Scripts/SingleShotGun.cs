using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.transform.CompareTag("Enemy"))
            {
                Health health = hit.transform.GetComponent<Health>();

                if(health == null)
                {
                    throw new System.Exception("Cannot find Health Component On Enemy");
                }
                else
                {
                    health.TakeDamage(((GunInfo)itemInfo).damage);
                }
            }
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        //finds colliders of stuff to set bullet thingy as a child of parent so it moves with it
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            //prefab, where it got hit + direction of object hit, direction of hit(z or where to face, which direction isup)
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal*0.01f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            //sets parents
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        
    }
}
