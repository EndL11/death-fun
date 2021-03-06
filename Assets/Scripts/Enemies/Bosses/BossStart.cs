﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            SoundMusicManager.instance.TriggerBossSoundPlay();
            GameSaving.instance.BossStartFight();
            Destroy(gameObject);
        }
    }
}
