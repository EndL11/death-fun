using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainMenu : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        //  get animator component
        anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        if (Input.GetMouseButton(0))        //  if pressed left mouse button
            Attack();        
    }

    private void Attack()
    {
        //  stop previous animation
        anim.SetBool("Attack", false);
        //  start new
        anim.SetBool("Attack", true);
    }
}
