using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void Destroy()
    {
        if(transform.parent.gameObject.CompareTag("Player"))
            GameSaving.instance.GameOver();
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

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void DeactivateShopNotification()
    {
        gameObject.SetActive(false);
    }
}
