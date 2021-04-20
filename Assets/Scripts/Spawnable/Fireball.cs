using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float speed = 7f;
    public GameObject particles;
    public float destroyDelay = 7f;
    private bool particlesSpawned = false;

    public float Damage
    {
        set { damage = value; }
    }

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (destroyDelay > 0.1f)
        {
            destroyDelay -= Time.deltaTime;
        }
        else
        {
            if (!particlesSpawned)
                Particles();

            particlesSpawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            Collision(collision.gameObject);
        }
    }

    public void Collision(GameObject collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            Vector2 pushBackDirection = transform.position.x > collision.transform.position.x ? Vector2.left : Vector2.right;
            collision.GetComponentInParent<IDamagable>().TakeDamage(damage, pushBackDirection);
        }
        Particles();
        Destroy(gameObject);
    }


    private void Particles()
    {
        //  spawn boom particles 
        GameObject spawnedParticles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(spawnedParticles, 1.5f);
    }
}
