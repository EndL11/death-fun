﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    public void StopAttack()
    {
        GetComponent<Animator>().SetBool("Attack", false);
    }

    public void Attack()
    {
        GetComponentInParent<Player>()?.ApplyAttack();
        //  set trigger to start idle animation 
        GetComponent<Animator>().SetTrigger("AttackNull");
    }
}