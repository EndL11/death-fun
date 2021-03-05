using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hp = 100f;
    private float speed = 3f;
    private Rigidbody2D rb;
    private Animator anim;

    private const float timeToChangeDirection = 5f;
    public float _timeToChangeDirection = timeToChangeDirection;

    public int direction = -1;

    [SerializeField] private Transform checkPlatformEndPoint;
    public LayerMask whatIsGround;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        transform.rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
    }

    void Update()
    {
        //if(_timeToChangeDirection > 0f)
        //{
        //    _timeToChangeDirection -= Time.deltaTime;
        //}
        //else
        //{
        //    _timeToChangeDirection = timeToChangeDirection;
        //}
        if (!isEndPlatform())
            Move();
        else
            ChangeMovementDirection();
    }

    public void ApplyDamage(float damage)
    {
        PushBack();
        hp -= damage;
        if (hp <= 0)
            DestroyEnemy();
    }

    private void PushBack()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(transform.position.x + 0.1f, transform.position.y + 5f) * .4f, ForceMode2D.Impulse);
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

    private void Move()
    {
        transform.Translate(transform.right * direction * speed * Time.deltaTime);
        anim.SetBool("Run", true);
    }
}
