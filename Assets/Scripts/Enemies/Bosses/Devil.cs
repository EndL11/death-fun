using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Devil : Boss
{
    public GameObject fireballPrefab;
    public Transform checkPlayerBack;
    public GameObject cage;

    protected override void DropChest() { }
    protected override void SpawnEnemyOnTakingDamage() { }
    protected override bool NeedToTurnAround()
    {
        return IsGrounded() && (IsEndPlatform() || IsWall() || IsPlayerBack());
    }
    protected override void OnDead()
    {
        base.OnDead();
        cage.SetActive(false);
    }

    protected override void Update()
    {
        if (IsDead() || !_canMove)
            return;

        if (!IsPlayerNear() && IsGrounded() && !IsEndPlatform() && !IsWall() && !IsPlayerBack())
            Move();
        else if (NeedToTurnAround())
            ChangeMovementDirection();

        //  if delay greater zero
        if (_attackDelay > 0f)
        {
            //  decrease delay
            _attackDelay -= Time.deltaTime;
            //  if player near, stop and wait (show idle anim)
            if (IsPlayerNear())
            {
                speed = 0f;
                StopRunAnimation();
            }
        }
        else
        {
            Attack();
        }
    }

    protected bool IsPlayerBack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerBack.position, playerCheckZone, whatToAttack);
        return colliders.Length != 0;
    }

    public override void MakeAttack()
    {
        if(IsDead())
            return;

        speed = _speed;
    }

    public override void Attack()
    {
        StartCoroutine(SpawnLineFireballs());
        //  attack
        PlayAttackAnimation();
        //  reset attack delay
        _attackDelay = attackDelay;
    }

    private void SpawnFireball()
    {
        SoundMusicManager.instance.FlameBossPlay();
        GameObject fireball = Instantiate(fireballPrefab, checkPlayerPoint.position, transform.GetChild(0).rotation);
        fireball.GetComponent<Fireball>().Damage = damage;
    }

    private IEnumerator SpawnLineFireballs()
    {
        speed = 0f;
        StopRunAnimation();
        for (int i = 0; i < 3; i++)
        {
            SpawnFireball();
            yield return new WaitForSeconds(0.3f);
        }
        speed = _speed;
        PlayRunAnimation();
    }
}

