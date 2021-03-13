using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxePendulum : MonoBehaviour
{
    public float damage = 15f;
    public float upForce = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().ApplyDamage(damage, transform.up * upForce);
        }
    }
}
