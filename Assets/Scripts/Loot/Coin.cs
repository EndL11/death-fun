using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable: MonoBehaviour
{
    public int score = 1;
    protected void AddScore()
    {
        if (GameSaving.instance != null)
            GameSaving.instance.AddScore(score);
        SoundMusicManager.instance.TakeCoinSoundPlay();
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
            Destroy(gameObject);
        }
    }
}
