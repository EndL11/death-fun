using System.Collections;
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
        GetComponentInParent<Player>().ApplyAttack();
        //  set trigger to start idle animation 
        GetComponent<Animator>().SetTrigger("AttackNull");
    }

    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }

    public void EnemyAttack()
    {
        GetComponentInParent<Enemy>().Attack();
        GetComponent<Animator>().SetTrigger("AttackNull");
    }

    public void EnableCollisions()
    {
        GetComponentInParent<Collider2D>().enabled = true;
    }

    public void DisableCollisions()
    {
        GetComponentInParent<Collider2D>().enabled = false;
    }

    public void SetIncreasedSawDamage()
    {
        GetComponentInParent<Saw>().SetIncreasedValues();
    }

    public void SetDefaultSawValues()
    {
        GetComponentInParent<Saw>().SetStaticValues();
    }
}
