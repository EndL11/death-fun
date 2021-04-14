using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEnemyAnimator
{
    void PlayAttackAnimation();
    void StopAttackAnimation();
    void PlayRunAnimation();
    void StopRunAnimation();
    void PlayDieAnimation();
}

public interface IBossAnimator : IEnemyAnimator
{
    void PlayBlockAnimation();
    void StopBlockAnimation();
}

public abstract class BaseEnemy : Character, IEnemyAnimator
{
    public EnemyAnalytics.Names enemyName;
    public enum MovementDirection { RIGHT = 1, LEFT = -1 };
    public GameObject soulPrefab;

    [Header("Movement")]
    #region Movement
    public float speed;
    protected float _speed;
    [SerializeField] protected MovementDirection _direction = MovementDirection.RIGHT;   //  -1 to left, 1 to right
    public LayerMask whatIsGround;
    public LayerMask whatToAvoid;
    public Transform wallChecker;
    public Transform endOfPlatformChecker;
    #endregion

    protected override void Start()
    {
        _healthManager.hp *= GameSaving.instance.difficultyCoefficient;
        _healthManager.maxHP = _healthManager.hp;
        _speed = speed;
        base.Start();
        transform.GetChild(0).rotation = Quaternion.Euler(0f, ((int)_direction < 0 ? 0f : 180f), 0f);
    }

    protected virtual void Update()
    {
        if (IsDead())
            return;

    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        SoundMusicManager.instance.PunchPlay();


        if (IsDead())
            OnDead();

        if (hurtParticles != null)
            hurtParticles.Play();

        StartCoroutine(HurtAnimation());
    }
    public override void TakeDamage(float damage, Vector2 pushBackDirection)
    {
        SoundMusicManager.instance.PunchPlay();
        base.TakeDamage(damage, pushBackDirection);
    }
    protected virtual bool NeedToTurnAround()
    {
        return IsGrounded() && (IsEndPlatform() || IsWall());
    }

    protected void ChangeMovementDirection()
    {
        //  set direction to another
        _direction = _direction == MovementDirection.RIGHT ? MovementDirection.LEFT : MovementDirection.RIGHT;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction < 0 ? 0f : 180f), 0f);
    }

    protected virtual void Move()
    {
        transform.Translate(speed * transform.right * (int)_direction * Time.deltaTime);
    }

    public void ChangeToRandomDirection()
    {
        int randNumber = UnityEngine.Random.Range(1, 3);
        if (randNumber == 1)
            _direction = MovementDirection.LEFT;
        else
            _direction = MovementDirection.RIGHT;

        transform.GetChild(0).rotation = Quaternion.Euler(0f, ((int)_direction < 0 ? 0f : 180f), 0f);
    }

    protected override void OnDead()
    {
        SoundMusicManager.instance.DeathPlay();
        SpawnSoul();
        GameSaving.instance.EnemyDead(gameObject);
    }

    protected virtual void SpawnSoul()
    {
        GameObject soul = Instantiate(soulPrefab, transform.position, Quaternion.identity);
        Destroy(soul, 1.5f);
    }

    protected bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, whatIsGround);
        return colliders.Length > 0;
    }

    protected bool IsWall()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallChecker.position, 0.1f, whatToAvoid);
        return colliders.Length > 0;
    }

    protected bool IsEndPlatform()
    {
        //  return true if platform is ended
        Collider2D[] colliders = Physics2D.OverlapCircleAll(endOfPlatformChecker.position, 0.3f, whatIsGround);
        return colliders.Length == 0;
    }

    public virtual void PlayAttackAnimation()
    {
        _anim.SetBool("Attack", true);
    }

    public virtual void StopAttackAnimation()
    {
        _anim.SetBool("Attack", false);
    }

    public virtual void PlayRunAnimation()
    {
        _anim.SetBool("Run", true);
    }

    public virtual void StopRunAnimation()
    {
        _anim.SetBool("Run", false);
    }

    public virtual void PlayDieAnimation()
    {
        _anim.SetTrigger("Die");
    }
}


public class Enemy : BaseEnemy
{
    public Transform checkPlayerPoint;
    public float playerCheckZone = 0.3f;

    public float attackDelay = 3f;
    protected float _attackDelay;

    protected override void Start()
    {
        base.Start();
        _attackDelay = attackDelay;
    }

    protected override void Update()
    {
        if (IsDead())
            return;

        if (!IsPlayerNear() && IsGrounded() && !IsEndPlatform() && !IsWall())
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
            //  if can attack and player in attack zone
            if (IsPlayerNear())
            {
                Attack();
            }
        }
    }

    protected override void Move()
    {
        //  if playing attack anim - return
        if (_anim.GetBool("Attack") || _anim.GetCurrentAnimatorStateInfo(0).IsName("AttackNull"))
            return;
        //  reset speed to normal
        speed = _speed;
        //  start run animation
        PlayRunAnimation();
        //  move enemy according to direction
        transform.Translate(transform.right * (int)_direction * speed * Time.deltaTime);
    }

    public override void Attack()
    {
        //  attack
        PlayAttackAnimation();
        //  reset attack delay
        _attackDelay = attackDelay;
    }

    public override void MakeAttack()
    {
        base.MakeAttack();
        speed = _speed;
        //StopAttackAnimation();
    }

    protected override void OnDead()
    {
        _rb.velocity = Vector2.zero;
        StopAttackAnimation();
        base.OnDead();
        PlayDieAnimation();
    }

    protected bool IsPlayerNear()
    {
        //  return true if in player check zone at least 1 player object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, playerCheckZone, whatToAttack);
        return colliders.Length != 0;
    }
}

