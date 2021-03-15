using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject playerPrefab;
    private PlayerMovement playerMovement;
    void Awake()
    {
        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        GameObject player = null;
        if (playerPrefab != null)
        {
             player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
             player.transform.localScale = Vector3.zero;
        }
        while (transform.localScale.x < 1f)
        {
            transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime, transform.localScale.y + Time.deltaTime, transform.localScale.z);
            if(player != null)
                player.transform.localScale = new Vector3(player.transform.localScale.x + Time.deltaTime, player.transform.localScale.y + Time.deltaTime, 1f);
            yield return null;
        }
        if(player != null)
        {
            StartCoroutine(EndAnimation());
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    public IEnumerator EndAnimation()
    {
        while(transform.localScale.x > 0f)
        {
            transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime, transform.localScale.y - Time.deltaTime, transform.localScale.z);
            yield return null;
        }
        if(playerMovement != null)
            playerMovement.CanMove = true;
        Destroy(gameObject);
    }
}
