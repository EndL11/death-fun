using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorFunctions : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void StopAttack()
    {
        _anim.SetBool("Attack", false);
        _anim.SetTrigger("AttackNull");
    }

    public void Attack()
    {
        IAttackable attackable = GetComponentInParent<IAttackable>();
        if (attackable != null) { attackable.MakeAttack(); return; }
        //  set trigger to start idle animation 
        _anim.SetTrigger("AttackNull");
    }

    public void Destroy()
    {
        if (transform.parent.gameObject.CompareTag("Player"))
            GameSaving.instance.GameOver();
        Destroy(transform.parent.gameObject);
    }

    public void EnemyAttack()
    {
        GetComponentInParent<IAttackable>().MakeAttack();
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

    public void ActivateBlockCollider()
    {
        transform.GetChild(transform.parent.childCount).GetComponent<Collider2D>().enabled = true;
    }

    public void BomberManDetonate()
    {
        GetComponentInParent<IAttackable>().MakeAttack();
    }

    public void StartRunBomber()
    {
        GetComponentInParent<Animator>().SetTrigger("Run");
    }

    public void StartBlickingBomber()
    {
        GetComponentInParent<Animator>().SetTrigger("Blick");
    }

}
