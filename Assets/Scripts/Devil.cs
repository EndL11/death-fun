using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Boss
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] protected Transform checkPlayerPointBack;
    public GameObject cage;
    public override void Attack()
    {
        //  do not attack if dead
        if (Dead) return;
        //  reset speed to normal
        speed = _speed;
    }

    private void SpawnFireball()
    {
		SoundMusicManager.instance.FlameBossPlay();
        GameObject fireball = Instantiate(fireballPrefab, checkPlayerPoint.position, transform.GetChild(0).rotation);
        fireball.GetComponent<Fireball>().Damage = damage;
    }

    protected override void Update()
    {
        if (!canMove || Dead)
            return;
        if (!isPlayerNear() && isGrounded() && !isEndPlatform() && !Dead && !isWall() && !isPlayerBack())
            Move();
        else if (!Dead && isGrounded() && (isEndPlatform() || isWall() || isPlayerBack()))
            ChangeMovementDirection();

        //  if delay greater zero
        if (_attackDelay > 0f)
        {
            //  decrease delay
            _attackDelay -= Time.deltaTime;
            //  if player near, stop and wait (show idle anim)
            if (isPlayerNear())
            {
                speed = 0f;
                anim.SetBool("Run", false);
            }
        }
        else
        {
            StartCoroutine(SpawnLineFireballs());
            //  attack
            anim.SetBool("Attack", true);
            //  reset attack delay
            _attackDelay = attackDelay;
        }
    }

    private IEnumerator SpawnLineFireballs()
    {
        speed = 0f;
        anim.SetBool("Run", false);
        for (int i = 0; i < 3; i++)
        {
            SpawnFireball();
            yield return new WaitForSeconds(0.3f);
        }
        speed = _speed;
        anim.SetBool("Run", true);
    }

    protected bool isPlayerBack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPointBack.position, playerCheckZone, whatIsPlayer);
        return colliders.Length != 0;
    }

    protected override void DestroyEnemy()
    {
        base.DestroyEnemy();
        cage.SetActive(false);
    }
}
