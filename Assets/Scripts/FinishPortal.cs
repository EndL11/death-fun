using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FinishPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            //  saving player stats
            collision.GetComponentInParent<Player>().SavePlayerStats();
            Destroy(collision.transform.parent.gameObject);
            StartCoroutine(WaitToEndAnimation());            
            //  if it's tutorial level not to save player stats
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                //  saving tutor complete
                GameSaving.instance.SaveCompleteTutorial();
                return;
            }

            PlayerPrefs.SetInt("@coins", GameSaving.instance.score);
        }
    }

    private IEnumerator WaitToEndAnimation()
    {
        StartCoroutine(GetComponent<Portal>().EndAnimation());
        while (transform.localScale.x > 0f)
        {
            yield return null;
        }
        Camera.main.GetComponent<Animator>().SetTrigger("Finish");
    }
}
