using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Sphere"))
        {
            collision.gameObject.GetComponentInParent<BlackHole>().Collision(GetComponent<Collider2D>());
        }
        else if (collision.gameObject.CompareTag("Fireball"))
        {
            collision.gameObject.GetComponentInParent<Fireball>().Collision(gameObject);
        }
    }
}
