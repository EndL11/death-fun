using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : BaseEnemy
{
    #region MovementBorders
    public Transform leftBorder;
    public Transform rightBorder;
    #endregion

    #region SpawnBomberSettings
    public GameObject bomberManPrefab;
    [Header("Min/Max attack delay")]
    public float min = 3f;
    public float max = 5f;
    private float _attackDelay;
    #endregion

    private bool _isSpawning = false;

    protected override void Start()
    {
        base.Start();
        _attackDelay = GetAttackDelay();
    }

    protected override void Update()
    {
        if(IsDead())
            return;

        if (NeedToTurnAround())
            ChangeMovementDirection();

        if (!IsDead() && !_isSpawning)
            Move();

        if (_attackDelay > 0f)
        {
            _attackDelay -= Time.deltaTime;
        }
        else
        {
            Attack();
            _attackDelay = GetAttackDelay();
        }
    }

    private float GetAttackDelay(){
        return Random.Range(min, max);
    }

    protected override bool NeedToTurnAround()
    {
        return ((int)_direction == 1 && transform.position.x > rightBorder.position.x)
        || ((int)_direction == -1 && transform.position.x < leftBorder.position.x);
    }

    protected override void Move()
    {
        PlayRunAnimation();
        base.Move();
    }

    public override void PlayRunAnimation()
    {
        _anim.SetBool("Fly", true);
    }

    public override void StopRunAnimation()
    {
        _anim.SetBool("Fly", false);
    }

    public override void Attack()
    {
        StartCoroutine(SpawnBomberMan());
    }

    protected override void OnDead()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        base.OnDead();
        PlayDieAnimation();
    }

    protected override void SpawnSoul()    {}

    private IEnumerator SpawnBomberMan()
    {
        _isSpawning = true;
        StopRunAnimation();
        yield return new WaitForSeconds(.3f);
        GameObject bombMan = Instantiate(bomberManPrefab, transform.position, Quaternion.identity);
        bombMan.GetComponent<BaseEnemy>().ChangeToRandomDirection();
        _isSpawning = false;
    }

}

