using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    //  force for jump
    [SerializeField] private float jumpForce = 10f;

    private Player player;

    //  transform of ground checker
    [SerializeField] private Transform groundChecker;
    //  layermask to set layer for ground
    public LayerMask whatIsGround;
    private bool canMove = false;
    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    private void Start()
    {
        //  get animator component
        anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (player.Dead || !canMove)
            return;
        //  if pressed left mouse button,, player not dead and not interact with UI
        if (Input.GetMouseButton(0) && !player.Dead && EventSystem.current.currentSelectedGameObject == null)        
            Attack();

        if (isGrounded() && Input.GetKey(KeyCode.W))
            Jump();

    }

    private void FixedUpdate()
    {
        if (player.Dead || !canMove)
            return;

        if (Input.GetAxis("Horizontal") != 0)   //  if moving to somewhere
            Move();
        else
            anim.SetBool("Run", false);         //  if not moving, stop run anim

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
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction > 0 ? 0f : 180f), 0f);
    }

    private void Attack()
    {
        //  stop previous animation
        anim.SetBool("Attack", false);
        //  start new
        anim.SetBool("Attack", true);
    }


    void Jump()
    {
        //  zeroing velocity (physics)
        rb.velocity = Vector2.zero;
        //  jumping by impulse player up
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool isGrounded()
    {
        //  get array of colliders with layer 'Ground'
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, 0.1f, whatIsGround);
        //  if there is al least one - return true (player on ground)
        return colliders.Length > 0;
    }
}
