using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject hintText;
    public GameObject showMenu;
    private Player player = null;
    private bool playerInZone = false;
    void Start()
    {
        hintText.SetActive(false);
        showMenu.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerInZone)
        {
            ShowShopMenu();
        }
    }

    private void ShowShopMenu()
    {
        showMenu.SetActive(true);
        hintText.SetActive(false);
    }

    public void ApplyItem(UpgradeItem item)
    {
        if (player == null)
            return;

        player.AddHealth(player.MAXHP * item.Value);
    }

    public void HideShopMenu()
    {
        showMenu.SetActive(false);
        hintText.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            playerInZone = true;
            hintText.SetActive(playerInZone);
            player = collision.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            playerInZone = false;
            showMenu.SetActive(playerInZone);
            hintText.SetActive(playerInZone);
            player = null;
        }
    }
}
