using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPrefab", 0, 5f);
    }


    private void SpawnPrefab()
    {
        if (spawnPoints.Length < 0) return;

        Instantiate(prefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
    }
}
