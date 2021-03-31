using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private int _score = 500;
    private bool _opened = false;
    public Text scoreText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            if (_opened)
                return;

            GameSaving.instance.AddScore(_score);
            scoreText.text = $"+{_score}";
            GetComponent<Animator>().SetTrigger("Open");
            _opened = true;
        }
    }
}
