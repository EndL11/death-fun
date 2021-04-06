using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable: MonoBehaviour
{
    public int score = 1;
    public AudioSource sfx;
    protected void AddScore()
    {
        if (GameSaving.instance != null)
            GameSaving.instance.AddScore(score);
    }
}


public class Coin : Lootable
{
    public GameObject particles;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            AddScore();
            GameObject spawnedParticles = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(spawnedParticles, 1.5f);
            
            if(sfx != null)
                sfx.Play();
            Destroy(gameObject);
        }
    }
}
