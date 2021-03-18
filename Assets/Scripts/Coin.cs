using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private int score = 1;
    public GameObject particles;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            if(GameSaving.instance != null)
                GameSaving.instance.AddScore(score);
            GameObject spawnedParticles = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(spawnedParticles, 1.5f);
            Destroy(gameObject);
        }
    }
}
