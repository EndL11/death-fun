using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberMan : BaseEnemy
{
    [SerializeField] private float radius = 2f;
    private bool _isDetonating = false;

    protected override void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        transform.GetChild(0).rotation = Quaternion.Euler(0f, ((int)_direction < 0 ? 0f : 180f), 0f);
    }

    protected override void Update() {
        base.Update();
         if (IsGrounded() && !IsWall())
            Move();
        else if (NeedToTurnAround())
            ChangeMovementDirection();
    }

    protected override bool NeedToTurnAround()
    {
        return IsGrounded() && IsWall();
    }

    protected override void Move()
    {
        transform.Translate(-transform.right * (int)_direction * speed * Time.deltaTime);
    }

    public override void MakeAttack()
    {
        Detonate();
    }

    public override void PlayDieAnimation()
    {
        _anim.SetTrigger("Boom");
    }

    public override void PlayAttackAnimation()  { }
    public override void StopAttackAnimation() { }
    public override void PlayRunAnimation()  { }
    public override void StopRunAnimation() { }
    private void Detonate()
    {
        if (_isDetonating)
            return;

        PlayDieAnimation();
        speed = 0f;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.velocity = Vector3.zero;

        SoundMusicManager.instance.ExplosionPlay();
        
        _isDetonating = true;
        Vector2 point = new Vector2(transform.position.x, transform.position.y + .3f);
        Collider2D collider = Physics2D.OverlapCircle(point, radius, whatToAttack);
        if (collider)
        {
            Vector2 directionToPush = transform.position.x > collider.transform.position.x ? Vector2.left : Vector2.right;
            collider.GetComponent<IDamagable>().TakeDamage(damage, directionToPush);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Detonate();
        }
    }

    public override void TakeDamage(float damage)
    {
        Detonate();
    }
    public override void TakeDamage(float damage, Vector2 pushBackDir)
    {
        Detonate();
    }
}


