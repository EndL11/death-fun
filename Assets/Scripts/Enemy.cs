using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float hp = 100f;
    public float speed = 3f;
    public float damage = 25f;
    private Rigidbody2D rb;
    private Animator anim;

    private const float timeToChangeDirection = 5f;
    public float _timeToChangeDirection = timeToChangeDirection;

    private int direction = -1;

    private bool dead = false;

    public float attackDelay = 0.5f;

    [SerializeField] private Transform checkPlatformEndPoint;
    [SerializeField] private Transform checkPlayerPoint;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        transform.rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
    }

    void Update()
    {
        if (!isPlayerNear() && isGrounded() && !isEndPlatform() && !dead)
            Move();
        else if(!dead && isGrounded() && isEndPlatform())
            ChangeMovementDirection();

        if (attackDelay > 0f)
        {
            attackDelay -= Time.deltaTime;
            if (isPlayerNear())
            {
                speed = 0f;
                anim.SetBool("Run", false);
            }
        }
        else
        {
            if (isPlayerNear())
            {
                anim.SetBool("Attack", false);
                anim.SetBool("Attack", true);
                speed = anim.GetBool("Attack") ? 0f : 3f;
            }
            attackDelay = 0.5f;
        } 
    }

    public void ApplyDamage(float damage, Vector2 dir)
    {
        hp -= damage;
        if (hp <= 0)
            DestroyEnemy();
        if(!dead)
            PushBack(dir);
    }

    private void PushBack(Vector2 dir)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir, ForceMode2D.Impulse);
        StartCoroutine(HurtAnimation());
    }


    private IEnumerator HurtAnimation()
    {
        Color c = GetComponentInChildren<SpriteRenderer>().material.color;
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    private void DestroyEnemy()
    {
        dead = true;
        anim.SetTrigger("Die");
    }

    private void ChangeMovementDirection()
    {
        direction = direction == 1 ? -1 : 1;
        transform.rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
    }

    private bool isEndPlatform()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlatformEndPoint.position, 0.3f, whatIsGround);
        return colliders.Length == 0;
    }

    private bool isGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, whatIsGround);
        return colliders.Length > 0;
    }

    private void Move()
    {
        if (anim.GetBool("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("AttackNull"))
            return;

        speed = 3f;
        anim.SetBool("Run", true);
        transform.Translate(transform.right * direction * speed * Time.deltaTime);
    }

    public void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, 0.3f, whatIsPlayer);
        Vector2 directionToPush = new Vector2((transform.position.x > checkPlayerPoint.position.x ? transform.position.x - 3f : transform.position.x + 3f), transform.position.y + 3f);
        foreach (var enemy in colliders)
        {
            enemy.GetComponent<Player>().ApplyDamage(damage, directionToPush);
        }
        speed = 3f;
        anim.SetBool("Attack", false);
    }

    private bool isPlayerNear()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, 0.3f, whatIsPlayer);
        return colliders.Length != 0;
    }
}
