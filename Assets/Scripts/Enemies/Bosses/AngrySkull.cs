using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngrySkull : Boss
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public float pushForce = 10f;

    protected override void SpawnEnemyOnTakingDamage() { }
    protected override void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _speed = speed;
        transform.GetChild(0).rotation = Quaternion.Euler(0f, ((int)_direction < 0 ? 0f : 180f), 0f);

        _healthManager.hp *= GameSaving.instance.difficultyCoefficient;
        _healthManager.maxHP = _healthManager.hp;

        _healthManager.healthBar = bossUI.GetComponentInChildren<Slider>();
        _healthStatsText = _healthManager.healthBar.GetComponentInChildren<Text>();

        _healthManager.healthBar.maxValue += _healthManager.maxHP;
        _healthManager.healthBar.value += _healthManager.hp;

        _attackDelay = attackDelay;
        _healthStatsText.text = $"{_healthManager.healthBar.value} / {_healthManager.healthBar.maxValue}";
        GameSaving.instance.OnBossStart += StartFight;
    }

    protected override void Update()
    {
        if (IsDead())
            return;

        if (_attackDelay > 0f)
        {
            _attackDelay -= Time.deltaTime;
        }
        else
        {
            StartCoroutine(SpawnEnemies());
            _attackDelay = attackDelay;
        }

        if (!IsEndPlatform() && IsGrounded())
            Move();
        else if (NeedToTurnAround())
            ChangeMovementDirection();
    }

    public override void PlayDieAnimation() { }
    public override void PlayRunAnimation() { }
    public override void StopRunAnimation() { }
    public override void StopAttackAnimation() { }

    protected override void Move()
    {
        transform.Translate(transform.right * (int)_direction * _speed * Time.deltaTime);
    }
    protected override void OnDead()
    {
        SpawnSoul();
        _healthManager.healthBar.maxValue -= _healthManager.maxHP;
        _healthStatsText.text = $"{_healthManager.healthBar.value} / {_healthManager.healthBar.maxValue}";
        dieSFX.Play();
        DropChest();

        if (_healthManager.healthBar.value <= 0f)
            bossUI.SetActive(false);

        GameSaving.instance.EnemyDead(gameObject);

        Destroy(gameObject);
    }

    protected override bool NeedToTurnAround()
    {
        return IsEndPlatform() && IsGrounded();
    }

    public override void TakeDamage(float damage, Vector2 pushBackDirection)
    {
        if (_healthManager.dead)
            return;

        _healthManager.hp -= damage;

        if (_healthManager.hp - damage <= 0f)
            _healthManager.healthBar.value -= _healthManager.hp;
        else
            _healthManager.healthBar.value -= damage;

        hurtSFX.Play();

        _healthStatsText.text = $"{_healthManager.healthBar.value} / {_healthManager.healthBar.maxValue}";
        if (_healthManager.hp <= 0f)
        {
            _healthManager.dead = true;
            OnDead();
        }

        if (!_healthManager.dead)
        {
            if (hurtParticles != null)
                hurtParticles.Play();

            PushBack(pushBackDirection);

            StartCoroutine(HurtAnimation());
        }
    }

    public override void PlayAttackAnimation()
    {
        _anim.SetTrigger("Spawn");
    }

    private IEnumerator SpawnEnemies()
    {
        PlayAttackAnimation();
        yield return new WaitForSeconds(.3f);
        Vector2 leftSpawnPoint = new Vector2(transform.position.x - 0.3f, transform.position.y);
        Vector2 rightSpawnPoint = new Vector2(transform.position.x + 0.3f, transform.position.y);
        Vector2 pushDirection = new Vector2(1f, .3f);

        GameObject leftEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], leftSpawnPoint, Quaternion.identity);
        leftEnemy.GetComponent<Rigidbody2D>().AddForce(-pushDirection * pushForce, ForceMode2D.Impulse);

        GameObject rightEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], rightSpawnPoint, Quaternion.identity);
        rightEnemy.GetComponent<Rigidbody2D>().AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

        GameObject centerEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
        centerEnemy.GetComponent<Rigidbody2D>().AddForce(transform.up * pushForce, ForceMode2D.Impulse);

        leftEnemy.GetComponent<BaseEnemy>().ChangeToRandomDirection();
        centerEnemy.GetComponent<BaseEnemy>().ChangeToRandomDirection();
        rightEnemy.GetComponent<BaseEnemy>().ChangeToRandomDirection();
    }
}

