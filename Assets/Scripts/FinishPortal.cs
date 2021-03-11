﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            collision.GetComponentInParent<Player>().SavePlayerStats();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}