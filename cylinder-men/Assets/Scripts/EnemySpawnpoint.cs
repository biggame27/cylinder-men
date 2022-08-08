using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour
{
    [SerializeField] GameObject graphics;

    void Awake()
    {
        graphics.SetActive(false);
    }
}
