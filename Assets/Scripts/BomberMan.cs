using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberMan : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float speed = 4f;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsGround;
    public LayerMask whatToAvoid;

    [SerializeField] private int direction = -1;

    public Transform checkGroundInFront;

    public EnemyAnalytics.Names _name;

    private bool detonating = false;

    public int Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    void Start()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction > 0 ? 0f : 180f), 0f);
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
        if (detonating)
            return;
		SoundMusicManager.instance.ExplosionPlay();
        detonating = true;
        speed = 0f;
        GetComponentInChildren<Animator>().SetTrigger("Boom");
        Vector2 point = new Vector2(transform.position.x, transform.position.y + .3f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, radius, whatIsPlayer);
        if(colliders.Length > 0)
        {
            //  calculating push direction
            Vector2 directionToPush = transform.position.x > colliders[0].transform.position.x ? Vector2.left : Vector2.right;
            colliders[0].GetComponent<Player>().ApplyDamage(damage, directionToPush);
        }
		
        GameSaving.instance.EnemyDead(gameObject);
    }

    private void ChangeMovementDirection()
    {
        direction = direction > 0 ? -1 : 1;
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction > 0 ? 0f : 180f), 0f);
    }

    private void Move()
    {
        transform.Translate(transform.right * direction * speed * Time.deltaTime);
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
