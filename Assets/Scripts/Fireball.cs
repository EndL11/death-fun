using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float speed = 7f;
    public GameObject particles;

    public float Damage
    {
        set { damage = value; }
    }

    private void Start()
    {
        Destroy(gameObject, 7f);
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            Vector2 pushBackDirection = transform.position.x > collision.transform.position.x ? Vector2.left : Vector2.right;
            collision.GetComponentInParent<Player>().ApplyDamage(damage, pushBackDirection);
            GameObject spawnedParticles = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(spawnedParticles, 1.5f);
            Destroy(gameObject);
        }
    }
}
