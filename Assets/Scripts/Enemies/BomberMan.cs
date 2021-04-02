using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberMan : MonoBehaviour
{
    [SerializeField] private float _damage = 30f;
    [SerializeField] private float _radius = 2f;
    [SerializeField] private float _speed = 4f;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsGround;
    public LayerMask whatToAvoid;

    [SerializeField] private int _direction = -1;

    public Transform checkGroundInFront;

    private bool _detonating = false;

    public int Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    void Start()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction > 0 ? 0f : 180f), 0f);
    }


    void Update()
    {
        if (IsGrounded() && !IsWall())
            Move();
        else if (IsGrounded() && IsWall())
            ChangeMovementDirection();
    }

    public void Detonate()
    {
        if (_detonating)
            return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector3.zero;
        SoundMusicManager.instance.ExplosionPlay();
        _detonating = true;
        _speed = 0f;
        GetComponentInChildren<Animator>().SetTrigger("Boom");
        Vector2 point = new Vector2(transform.position.x, transform.position.y + .3f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, _radius, whatIsPlayer);
        if(colliders.Length > 0)
        {
            //  calculating push direction
            Vector2 directionToPush = transform.position.x > colliders[0].transform.position.x ? Vector2.left : Vector2.right;
            colliders[0].GetComponent<Player>().ApplyDamage(_damage, directionToPush);
        }
    }

    private void ChangeMovementDirection()
    {
        _direction = _direction > 0 ? -1 : 1;
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction > 0 ? 0f : 180f), 0f);
    }

    private void Move()
    {
        transform.Translate(transform.right * _direction * _speed * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, whatIsGround);
        return colliders.Length > 0;
    }

    private bool IsWall()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkGroundInFront.position, 0.25f, whatToAvoid);
        return colliders.Length > 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Detonate();
        }
    }
}
