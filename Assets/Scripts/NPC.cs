using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    //  hint text (example 'press E')
    public GameObject hintText;
    //  npc menu
    public GameObject shopMenu;
    //  player reference
    private Player player = null;
    //  flag is player near to npc
    private bool playerInZone = false;

    //  current selected item
    private UpgradeItem selected = null;

    //  description text of selected item
    public Text descriptionText;

    //  notification of not enough money to buy something
    private GameObject notification;

    void Start()
    {
        //  hide hint text and menu
        hintText.SetActive(false);
        shopMenu.SetActive(false);
        notification = shopMenu.transform.GetChild(1).gameObject;
        notification.SetActive(false);
    }

    void Update()
    {
        //  if pressed E and player near - open shop window
        if(Input.GetKeyDown(KeyCode.E) && playerInZone)
        {
            ShowShopMenu();
        }
    }

    private void ShowShopMenu()
    {
        //  hiding hint text and showing shop window
        shopMenu.SetActive(true);
        hintText.SetActive(false);
    }

    public void ApplyItem()
    {
        //  if player is not near
        if (player == null || selected == null)
            return;

        if(GameSaving.instance.score < selected.Cost)
        {
            //  show notification
            notification.SetActive(true);
            return;
        }
        //  TODO: add switch to check is it adding hp or damage or something else
        player.AddHealth(player.MAXHP * selected.Value);
        GameSaving.instance.Buy(selected.Cost);
    }

    public void HideShopMenu()
    {
        shopMenu.SetActive(false);
        hintText.SetActive(true);
        descriptionText.text = "";
        selected = null;
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
            HideShopMenu();
            player = null;
        }
    }

    public void onPressItem(UpgradeItem item)
    {
        selected = item;
        descriptionText.text = $"{item.Description} - {item.Cost}$";
    }
}
