using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private int score = 500;
    private bool opened = false;
    [SerializeField] private Text scoreText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            if (opened)
                return;

            GameSaving.instance.AddScore(score);
            scoreText.text = $"+{score}";
            GetComponent<Animator>().SetTrigger("Open");
            opened = true;
        }
    }
}
