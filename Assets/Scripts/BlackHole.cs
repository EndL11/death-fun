using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float damage = 20f;
    public float radius = 1.1f;
    public LayerMask whatIsEnemy;
    public GameObject boomEffect;
    private List<GameObject> enemies = new List<GameObject>();
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(Vector2.right * 3f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyTrigger"))
        {
            if (collision.GetComponentInParent<Enemy>().Dead)
                return;

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SpawnBoomParticles();
        Vector2 pushDirection = transform.rotation.y < 90f ? Vector2.right : Vector2.left;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
        foreach (var enemy in colliders)
        {
            if (!enemies.Contains(enemy.gameObject) && !enemy.isTrigger)
            {
                enemies.Add(enemy.gameObject);
                enemy.GetComponent<Enemy>().ApplyDamage(damage, pushDirection);
            }
        }
        enemies.Clear();
    }

    private void SpawnBoomParticles()
    {
        GameObject boomParticles = Instantiate(boomEffect, transform.position, Quaternion.identity);
        Destroy(boomParticles, 1.5f);
    }
}
