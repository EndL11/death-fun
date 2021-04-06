using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStart : MonoBehaviour
{
    public AudioSource triggerBossSFX;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            triggerBossSFX.Play();
            GameSaving.instance.BossStartFight();
            Destroy(gameObject);
        }
    }
}
