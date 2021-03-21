using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaman : MonoBehaviour
{
    //  hint text (example 'text')
    public GameObject hintText;
    //  flag is player near to npc
    private bool playerInZone = false;

    void Start()
    {
        //  hide hint text
        hintText.SetActive(false);        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            //  if player enter to npc collider 
            playerInZone = true;
            hintText.SetActive(playerInZone);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            //  if player exit from npc collider
            playerInZone = false;
            hintText.SetActive(playerInZone);
        }
    }
}
