using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traider : NPC
{
    private Animator _anim;

    protected override void Start()
    {
        base.Start();
        _anim = GetComponentInChildren<Animator>();
    }

    public override void HideShopMenu()
    {
        _anim.SetTrigger("Idle");
        base.HideShopMenu();
    }

    protected override IEnumerator ShowShopMenu()
    {
        _anim.ResetTrigger("Idle");
        _anim.SetTrigger("Action");
        yield return new WaitForSeconds(1.8f);
        yield return base.ShowShopMenu();
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        _anim.SetTrigger("Idle");
        base.OnTriggerExit2D(collision);
    }
}
