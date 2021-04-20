using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBoss : Boss, IBossAnimator
{
    private bool _isBlocking = false;
    public float blockingTime = 5f;
    public Collider2D blockCollider;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        StartBlock();
    }
    public override void TakeDamage(float damage, Vector2 pushBackDirection)
    {
        if(_isBlocking)
            return;
            
        base.TakeDamage(damage, pushBackDirection);
        StartBlock();
    }
    private void StartBlock()
    {
        int rand = UnityEngine.Random.Range(1, 6);
        if (rand <= 2)
            StartCoroutine(Block());
    }
    public void PlayBlockAnimation()
    {
        _anim.SetBool("Block", true);
    }

    public void StopBlockAnimation()
    {
        _anim.SetBool("Block", false);
    }

    private IEnumerator Block()
    {
        _isBlocking = true;
        PlayBlockAnimation();
        _rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(blockingTime);
        StopBlockAnimation();
        _isBlocking = false;
        blockCollider.enabled = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    protected override void Move()
    {
        if (_isBlocking)
            return;

        base.Move();
    }
}
