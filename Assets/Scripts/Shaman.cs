using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaman : MonoBehaviour
{
    //  hint text (example 'text')
    public GameObject hintText;
    //  player reference
    private Player player = null;
    //  flag is player near to npc
    private bool playerInZone = false;

    void Start()
    {
        //  hide hint text
        hintText.SetActive(false);
        
    }

    public void HideShopMenu()
    {
        hintText.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            //  if player enter to npc collider 
            playerInZone = true;
            hintText.SetActive(playerInZone);
            player = collision.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            //  if player exit from npc collider
            playerInZone = false;
            hintText.SetActive(playerInZone);
            player = null;
        }
    }
}
