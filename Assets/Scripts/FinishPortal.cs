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
            //  load next scene
            
        }
    }

    private IEnumerator WaitToEndAnimation()
    {
        StartCoroutine(GetComponent<Portal>().EndAnimation());
        while (transform.localScale.x > 0f)
        {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
