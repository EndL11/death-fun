using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float hp = 100f;
    [SerializeField] protected float maxHP = 100f;
    public float speed = 3f;
    protected float _speed;
    //  movement direction - 1 - right, -1 - left
    [SerializeField] private int direction = -1;
    public float damage = 25f;
    private bool dead = false;
    //  attack delay
    [SerializeField] protected float attackDelay = 3f;
    protected float _attackDelay;

    public float attackZone = 0.5f;
    public float playerCheckZone = 0.3f;

    protected Rigidbody2D rb;
    protected Animator anim;

    public ParticleSystem hurtParticles;
    public GameObject soulPrefab;

    [SerializeField] private Transform checkPlatformEndPoint;
    [SerializeField] protected Transform checkPlayerPoint;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public LayerMask whatAvoid;

    public EnemyAnalytics.Names _name;
    public Slider healthBar = null;
    //  start color
    private Color c;

    public bool Dead
    {
        get { return dead; }
    }

    public int Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        //  setting rotating to sprite based on direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
        //  setting max hp to health bar
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
        _attackDelay = attackDelay;
        //  save start color
        c = GetComponentInChildren<SpriteRenderer>().material.color;
        _speed = speed;
    }

    protected virtual void Update()
    {
        if (dead)
            return;

        if (!isPlayerNear() && isGrounded() && !isEndPlatform() && !dead && !isWall())
            Move();
        else if(!dead && isGrounded() && (isEndPlatform() || isWall()))
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
            //  if can attack and player in attack zone
            if (isPlayerNear())
            {
                //  attack
                anim.SetBool("Attack", true);
                //  reset attack delay
                _attackDelay = attackDelay;
            }
        } 
    }

    public virtual void ApplyDamage(float damage, Vector2 dir)
    {
        if (dead)
            return;

        hp -= damage;
		SoundMusicManager.instance.PunchPlay();
        //  update health bar
        healthBar.value = hp;
        if (hp <= 0)
            DestroyEnemy();
        if (!dead)
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
        rb.velocity = Vector2.zero;
        //  push back enemy
        rb.AddForce(dir, ForceMode2D.Impulse);
    }


    private IEnumerator HurtAnimation()
    {
        // set sprite color to red         
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        //  wait 0.2 seconds
        yield return new WaitForSeconds(0.2f);
        //  set start color
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    protected virtual void DestroyEnemy()
    {
        //  stop attack animation
        anim.SetBool("Attack", false);
        dead = true;
        //  hide health bar (it's empty)
        healthBar.gameObject.SetActive(false);
        //  spawn soul
        SpawnSoul();
        if(GameSaving.instance != null)
            GameSaving.instance.EnemyDead(gameObject);
        //  show die animation
        anim.SetTrigger("Die");
    }

    private void SpawnSoul()
    {
        GameObject soul = Instantiate(soulPrefab, transform.position, Quaternion.identity);
        Destroy(soul, 1.5f);
    }

    protected void ChangeMovementDirection()
    {
        //  set direction to another
        direction = direction == 1 ? -1 : 1;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
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
        if (anim.GetBool("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("AttackNull"))
            return;
        //  reset speed to normal
        speed = _speed;
        //  start run animation
        anim.SetBool("Run", true);
        //  move enemy according to direction
        transform.Translate(transform.right * direction * speed * Time.deltaTime);
    }

    public virtual void Attack()
    {
        //  do not attack if dead
        if (dead) return;
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
