using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody2D _rb;
    [SerializeField] private float _speed = 5f;
    //  force for jump
    [SerializeField] private float _jumpForce = 10f;

    private Player _player;

    //  transform of ground checker
    [SerializeField] private Transform groundChecker;
    //  layermask to set layer for ground
    public LayerMask whatIsGround;
    private bool _canMove = false;
    public bool CanMove
    {
        get { return _canMove; }
        set { _canMove = value; }
    }

    private void Start()
    {
        //  get animator component
        _anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.Dead || !_canMove)
            return;
        //  if pressed left mouse button,, player not dead and not interact with UI
        if (Input.GetKey(KeyCode.Comma) && !_player.Dead && Time.timeScale == 1)        
            Attack();

        if (isGrounded() && Input.GetKey(KeyCode.W))
            Jump();

    }

    private void FixedUpdate()
    {
        if (_player.Dead || !_canMove)
            return;

        if (Input.GetAxis("Horizontal") != 0)   //  if moving to somewhere
            Move();
        else
            _anim.SetBool("Run", false);         //  if not moving, stop run anim

    }

    private void Move()
    {
        //  stop previous animation
        _anim.SetBool("Run", false);
        //  start new
        _anim.SetBool("Run", true);
        //  get direction - left or right
        float direction = Input.GetAxis("Horizontal");
        //  move player according to direction
        transform.Translate(transform.right * direction * _speed * Time.deltaTime);
        //  flip x if moving left
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction > 0 ? 0f : 180f), 0f);
    }

    private void Attack()
    {
        //  stop previous animation
        _anim.SetBool("Attack", false);
        //  start new
        _anim.SetBool("Attack", true);
		
    }


    void Jump()
    {
        //  zeroing velocity (physics)
        _rb.velocity = Vector2.zero;
        //  jumping by impulse player up
        _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
    }

    private bool isGrounded()
    {
        //  get array of colliders with layer 'Ground'
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, 0.1f, whatIsGround);
        //  if there is al least one - return true (player on ground)
        return colliders.Length > 0;
    }
}
