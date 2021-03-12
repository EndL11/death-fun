using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private int score = 0;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //GetComponent<CircleCollider2D>().enabled = false;
            gameObject.SetActive(false);
            score++;
            Debug.Log(score);
        }
    }
    private int GetScore()
    {
        return score;
    }
}
