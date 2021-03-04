using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private float speed = 3f;

    private void Start()
    {
        //  get animator component
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)   //  if moving to somewhere
            Move();
        else
            anim.SetBool("Run", false);         //  if not moving, stop run anim
        if (Input.GetMouseButtonDown(0))        //  if pressed left mouse button
            Attack();
        if (Input.GetMouseButtonDown(1))        //  if pressed right mouse button
            Die();
    }

    private void Move()
    {
        //  stop previous animation
        anim.SetBool("Run", false);
        //  start new
        anim.SetBool("Run", true);
        //  get direction - left or right
        float direction = Input.GetAxis("Horizontal");
        //  move player according to direction
        transform.Translate(transform.right * direction * speed * Time.deltaTime);
        //  flip x if moving left
        GetComponentInChildren<SpriteRenderer>().flipX = direction > 0;
    }

    private void Attack()
    {
        //  stop previous animation
        anim.SetBool("Attack", false);
        //  start new
        anim.SetBool("Attack", true);
    }

    private void Die()
    {
        anim.SetTrigger("Die");
    }
}
