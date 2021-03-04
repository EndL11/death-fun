using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private float speed = 3f;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
            Move();
        if (Input.GetMouseButtonDown(0))
            Attack();
        if (Input.GetMouseButtonDown(1))
            Die();
    }

    private void Move()
    {
        //  get direction - left or right
        float direction = Input.GetAxis("Horizontal");
        //  move player according to direction
        transform.Translate(transform.right * direction*  speed * Time.deltaTime);
        //  flip x if moving left
        GetComponentInChildren<SpriteRenderer>().flipX = direction > 0;

    }

    private void Attack()
    {
        anim.SetBool("Attack", false);
        anim.SetBool("Attack", true);
    }

    private void Die()
    {
        anim.SetTrigger("Die");
    }
}
