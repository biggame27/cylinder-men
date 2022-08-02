using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //referenced from anywhere
    public static SpawnManager instance;

    Spawnpoint[] spawnpoints;
    EnemySpawnpoint[] enemySpawnpoints;

    private int enemyCount;
    [SerializeField] private int maxEnemies;

    void Awake()
    {
        instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
        enemySpawnpoints = GetComponentsInChildren<EnemySpawnpoint>();

    }

    public Transform GetSpawnpoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }

    public Transform EnemyGetSpawnpoint()
    {
        return enemySpawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }

    public int getEnemies()
    {
        return enemyCount;
    }

    public void addEnemies()
    {
        enemyCount++;
    }
}
