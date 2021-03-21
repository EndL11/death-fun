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

    //  if has, play it before opening shop
    [SerializeField] private bool hasActionAnimation = false;

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
            StartCoroutine(ShowShopMenu());
        }
    }

    private IEnumerator ShowShopMenu()
    {
        //  play animation if has
        if (hasActionAnimation)
        {
            GetComponentInChildren<Animator>().ResetTrigger("Idle");
            GetComponentInChildren<Animator>().SetTrigger("Action");
            yield return new WaitForSeconds(1.8f);
        }
        yield return null;

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

        if(selected.Identificator == UpgradeItem.STATS.HP)
            player.AddHealth(selected.Value);
        else if (selected.Identificator == UpgradeItem.STATS.HP_HALF)
            player.AddHealth(player.MAXHP/2);
        else if (selected.Identificator == UpgradeItem.STATS.HP_FULL)
            player.AddHealth(player.MAXHP);
        else if(selected.Identificator == UpgradeItem.STATS.MAXHP)
            player.AddMaxHP(selected.Value);
        else if(selected.Identificator == UpgradeItem.STATS.DAMAGE)
            player.AddDamage(selected.Value);

        else if (selected.Identificator == UpgradeItem.STATS.SPHERE_DAMAGE)
            player.IncreaseSphereDamage(selected.Value);
        else if (selected.Identificator == UpgradeItem.STATS.SPHERE_DELAY && player.blackHoleDelay > 2f)
            player.DecreaseSphereDelay(selected.Value);
        else if (selected.Identificator == UpgradeItem.STATS.SPHERE_RADIUS)
            player.IncreaseSphereRadius(selected.Value);

        GameSaving.instance.Buy(selected.Cost);
    }

    public void HideShopMenu()
    {
        if (hasActionAnimation)
        {
            GetComponentInChildren<Animator>().SetTrigger("Idle");
        }
        notification.SetActive(false);
        shopMenu.SetActive(false);
        hintText.SetActive(true);
        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
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
            if (hasActionAnimation)
            {
                GetComponentInChildren<Animator>().SetTrigger("Idle");
            }
            //  if player exit from npc collider
            playerInZone = false;
            shopMenu.SetActive(false);
            hintText.SetActive(false);
            notification.SetActive(false);
            SetDefaultValues();
            player = null;
        }
    }

    public void onPressItem(UpgradeItem item)
    {
        selected = item;
        descriptionText.text = $"{item.Description} - {item.Cost}$";
    }
}
