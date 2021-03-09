using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float damage = 20f;
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

            Vector2 pushDirection = transform.rotation.y < 90f ? Vector2.right : Vector2.left;
            collision.GetComponentInParent<Enemy>().ApplyDamage(damage, pushDirection);
            Destroy(gameObject);
        }
    }
}
