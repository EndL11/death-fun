using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHP = 100f;
    public float speed = 3f;
    private float _speed;
    //  movement direction - 1 - right, -1 - left
    [SerializeField] private int direction = -1;
    public float damage = 25f;
    private bool dead = false;
    [SerializeField] private float attackDelay = 3f;
    private float _attackDelay;

    public float attackZone = 0.5f;
    public float playerCheckZone = 0.3f;

    private Rigidbody2D rb;
    private Animator anim;

    //  for future AI, to changing movement or staying 
    //private const float timeToChangeDirection = 5f;
    //public float _timeToChangeDirection = timeToChangeDirection;

    public ParticleSystem hurtParticles;

    [SerializeField] private Transform checkPlatformEndPoint;
    [SerializeField] private Transform checkPlayerPoint;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [SerializeField] private Slider healthBar;

    private Color c;

    public bool Dead
    {
        get { return dead; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        //  setting rotating to sprite based on direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
        //  setting masx hp to health bar
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
        _attackDelay = attackDelay;
        //  save start color
        c = GetComponentInChildren<SpriteRenderer>().material.color;
        _speed = speed;
    }

    void Update()
    {
        if (!isPlayerNear() && isGrounded() && !isEndPlatform() && !dead)
            Move();
        else if(!dead && isGrounded() && isEndPlatform())
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

    public void ApplyDamage(float damage, Vector2 dir)
    {
        hp -= damage;
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

    private void PushBack(Vector2 dir)
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

    private void DestroyEnemy()
    {
        //  stop attack animation
        anim.SetBool("Attack", false);
        dead = true;
        //  hide health bar (it's empty)
        healthBar.gameObject.SetActive(false);
        //  show die animation
        anim.SetTrigger("Die");
    }

    private void ChangeMovementDirection()
    {
        //  set direction to another
        direction = direction == 1 ? -1 : 1;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
    }

    private bool isEndPlatform()
    {
        //  return true if platform is ended
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlatformEndPoint.position, 0.3f, whatIsGround);
        return colliders.Length == 0;
    }

    private bool isGrounded()
    {
        //  return true if enemy is on ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, whatIsGround);
        return colliders.Length > 0;
    }

    private void Move()
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

    public void Attack()
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
        anim.SetBool("Attack", false);
    }

    private bool isPlayerNear()
    {
        //  return true if in player check zone at least 1 player object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, playerCheckZone, whatIsPlayer);
        return colliders.Length != 0;
    }
}
