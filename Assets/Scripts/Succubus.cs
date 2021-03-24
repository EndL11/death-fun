﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succubus : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            PlayerPrefs.DeleteKey("@level");
            Camera.main.GetComponent<Animator>().SetTrigger("Finish");
        }
    }
}
