using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainMenu : MonoBehaviour
{
    private Animator _anim;

    void Start()
    {
        //  get animator component
        _anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        if (Input.GetMouseButton(0))        //  if pressed left mouse button
            Attack();        
    }

    private void Attack()
    {
        //  stop previous animation
        _anim.SetBool("Attack", false);
        //  start new
        _anim.SetBool("Attack", true);
    }
}
