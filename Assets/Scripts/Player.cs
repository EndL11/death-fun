using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private float damage = 15f;
    //  gameobject to spawn (blackhole)
    [SerializeField] private GameObject blackHolePrefab;
    //  position for spawning black holes
    [SerializeField] private Transform spawnPosition;

    public LayerMask enemiesMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))        //  if pressed right mouse button
            SpawnBlackHole();
    }

    private void SpawnBlackHole()
    {
        Instantiate(blackHolePrefab, spawnPosition.position, transform.rotation);
    }

    public void ApplyAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition.position, 0.3f, enemiesMask);
        foreach (var enemy in colliders)
        {
            enemy.GetComponent<Enemy>().ApplyDamage(damage);
        }
    }
}
