using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform[] spawnPoints;

    void Start()
    {
        //  invoke function each 5 seconds
        InvokeRepeating("SpawnPrefab", 0, 5f);
    }


    private void SpawnPrefab()
    {
        //  if point or prefabs is zero - not to spawn
        if (spawnPoints.Length < 0 || prefabs.Length < 0) return;
        //  spawn random prefab in random point
        Instantiate(prefabs[Random.Range(0, prefabs.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
    }
}
