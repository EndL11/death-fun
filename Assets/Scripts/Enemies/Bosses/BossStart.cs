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
            Hide();
            Destroy(gameObject, 1.5f);
        }
    }

    private void Hide()
    {
        transform.localScale = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }
}
