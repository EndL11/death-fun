using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _hp = 100f;
    [SerializeField] protected float _maxHP = 100f;
    public float speed = 3f;
    protected float _speed;
    //  movement direction - 1 - right, -1 - left
    [SerializeField] private int _direction = -1;
    public float damage = 25f;
    private bool _dead = false;
    //  attack delay
    public float attackDelay = 3f;
    protected float _attackDelay;

    public float attackZone = 0.5f;
    public float playerCheckZone = 0.3f;

    protected Rigidbody2D _rb;
    protected Animator _anim;

    public ParticleSystem hurtParticles;
    public GameObject soulPrefab;

    [SerializeField] private Transform checkPlatformEndPoint;
    [SerializeField] protected Transform checkPlayerPoint;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public LayerMask whatAvoid;

    public EnemyAnalytics.Names enemyName;
    public Slider healthBar = null;
    //  start color
    private Color _color;

    public bool Dead
    {
        get { return _dead; }
    }

    public int Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        //  setting rotating to sprite based on direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction < 0 ? 0f : 180f), 0f);
        //  setting max hp to health bar
        healthBar.maxValue = _maxHP;
        healthBar.value = _hp;
        _attackDelay = attackDelay;
        //  save start color
        _color = GetComponentInChildren<SpriteRenderer>().material.color;
        _speed = speed;
    }

    protected virtual void Update()
    {
        if (_dead)
            return;

        if (!isPlayerNear() && isGrounded() && !isEndPlatform() && !_dead && !isWall())
            Move();
        else if(!_dead && isGrounded() && (isEndPlatform() || isWall()))
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
                _anim.SetBool("Run", false);
            }
        }
        else
        {
            //  if can attack and player in attack zone
            if (isPlayerNear())
            {
                //  attack
                _anim.SetBool("Attack", true);
                //  reset attack delay
                _attackDelay = attackDelay;
            }
        } 
    }

    public virtual void ApplyDamage(float damage, Vector2 dir)
    {
        if (_dead)
            return;

        _hp -= damage;
		SoundMusicManager.instance.PunchPlay();
        //  update health bar
        healthBar.value = _hp;
        if (_hp <= 0)
            DestroyEnemy();
        if (!_dead)
        {
            //  show particles
            hurtParticles.Play();
            //  push enemy back
            PushBack(dir);
            //  play hurt animation
            StartCoroutine(HurtAnimation());
        }
    }

    protected virtual void PushBack(Vector2 dir)
    {
        //  reset velocity
        _rb.velocity = Vector2.zero;
        //  push back enemy
        _rb.AddForce(dir, ForceMode2D.Impulse);
    }


    private IEnumerator HurtAnimation()
    {
        // set sprite color to red         
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        //  wait 0.2 seconds
        yield return new WaitForSeconds(0.2f);
        //  set start color
        GetComponentInChildren<SpriteRenderer>().material.color = _color;
    }

    protected virtual void DestroyEnemy()
    {
        //  stop attack animation
        _anim.SetBool("Attack", false);
        _dead = true;
		SoundMusicManager.instance.DeathPlay();
        //  hide health bar (it's empty)
        healthBar.gameObject.SetActive(false);
        //  spawn soul
        SpawnSoul();
        if(GameSaving.instance != null)
            GameSaving.instance.EnemyDead(gameObject);
        //  show die animation
        _anim.SetTrigger("Die");
    }

    private void SpawnSoul()
    {
        GameObject soul = Instantiate(soulPrefab, transform.position, Quaternion.identity);
        Destroy(soul, 1.5f);
    }

    protected void ChangeMovementDirection()
    {
        //  set direction to another
        _direction = _direction == 1 ? -1 : 1;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction < 0 ? 0f : 180f), 0f);
    }

    protected bool isEndPlatform()
    {
        //  return true if platform is ended
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlatformEndPoint.position, 0.3f, whatIsGround);
        return colliders.Length == 0;
    }

    protected bool isGrounded()
    {
        //  return true if enemy is on ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, whatIsGround);
        return colliders.Length > 0;
    }

    protected virtual void Move()
    {
        //  if playing attack anim - return
        if (_anim.GetBool("Attack") || _anim.GetCurrentAnimatorStateInfo(0).IsName("AttackNull"))
            return;
        //  reset speed to normal
        speed = _speed;
        //  start run animation
        _anim.SetBool("Run", true);
        //  move enemy according to direction
        transform.Translate(transform.right * _direction * speed * Time.deltaTime);
    }

    public virtual void Attack()
    {
        //  do not attack if dead
        if (_dead) return;
        //  get colliders of all players
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, attackZone, whatIsPlayer);
        //  calculating push direction
        Vector2 directionToPush = transform.position.x > checkPlayerPoint.position.x ? Vector2.left : Vector2.right;
        foreach (var player in colliders)
        {
            //  damage all players
            player.GetComponent<Player>().ApplyDamage(damage, directionToPush);
        }
        //  reset speed to normal
        speed = _speed;
        //  stop playing attack animation
        //anim.SetBool("Attack", false);
    }

    protected bool isPlayerNear()
    {
        //  return true if in player check zone at least 1 player object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, playerCheckZone, whatIsPlayer);
        return colliders.Length != 0;
    }

    protected bool isWall()
    {
        //  return true if in player check zone at least 1 avoid object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, 0.1f, whatAvoid);
        return colliders.Length > 0;
    }
}
