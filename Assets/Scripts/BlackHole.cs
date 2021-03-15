using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float damage = 20f;
    public float radius = 1.1f;
    public LayerMask whatIsEnemy;
    public ParticleSystem boomEffect;
    //  list of damaged enemies
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
        //  play boom particles
        boomEffect.Play();
        //  calculating direction to push enemy
        Vector2 pushDirection = transform.rotation.y < 90f ? Vector2.right : Vector2.left;
        //  get enemies at damage zone
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
        foreach (var enemy in colliders)
        {
            //  if enemy doesn't damaged and it's not trigger collider, add it to damaged enemies and damage
            if (!enemies.Contains(enemy.gameObject) && !enemy.isTrigger)
            {
                enemies.Add(enemy.gameObject);
                enemy.GetComponent<Enemy>().ApplyDamage(damage, pushDirection);
            }
        }
        enemies.Clear();
    }
}
