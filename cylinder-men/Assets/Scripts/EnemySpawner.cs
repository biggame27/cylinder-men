using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject zombiePrefab;
    public string zombiePrefabName;

    [HideInInspector] public Transform[] enemySpawnPoints;

    public float spawnDuration = 5f;

    public int zombieCount = 0;

    public const int maxZombies = 15;

    GameObject[] players;

    void Start()
    {
        enemySpawnPoints = new Transform[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            enemySpawnPoints[i] = transform.GetChild(i);
        }
        if(!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(CoStartSpawning());
    }

    void GetCurrentPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    IEnumerator CoStartSpawning()
    {
        yield return new WaitForSeconds(spawnDuration);
        GetCurrentPlayers();
        while(true)
        {
            
            zombieCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            
            for(int i = 0; i < enemySpawnPoints.Length; i++)
            {
                Transform enemySpawnPoint = enemySpawnPoints[i];
                if(zombieCount < maxZombies)
                {
                    zombieCount++;
                    //Instantiate(zombiePrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
                    GameObject enemyObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", zombiePrefabName), enemySpawnPoint.position, enemySpawnPoint.rotation, 0);
                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    enemy.players = players;
                }
            }
            yield return new WaitForSeconds(spawnDuration);
        }
        
    }

    public void startSpawning()
    {
        StartCoroutine(CoStartSpawning());
    }

}
