using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : Lootable
{
    private bool _opened = false;
    public Text scoreText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            if (_opened)
                return;
            AddScore();
            scoreText.text = $"+{score}";
            GetComponent<Animator>().SetTrigger("Open");
            _opened = true;
        }
    }
}
