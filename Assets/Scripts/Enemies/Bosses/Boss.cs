using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    protected Text _healthStatsText;

    [Header("Spawnable enemy on taking damage")]
    public GameObject spawnableEnemy;
    public float upForce = 5f;
    protected bool _canMove = false;
    public GameObject bossUI;
    public GameObject chest;

    protected override void Start()
    {
        _healthManager.healthBar = bossUI.GetComponentInChildren<Slider>();
        _healthStatsText = _healthManager.healthBar.GetComponentInChildren<Text>();
        base.Start();
        _healthStatsText.text = $"{_healthManager.hp} / {_healthManager.maxHP}";
        GameSaving.instance.OnBossStart += StartFight;
    }

    public override void TakeDamage(float damage, Vector2 pushBackDirection)
    {
        base.TakeDamage(damage, pushBackDirection);
        _healthStatsText.text = $"{_healthManager.hp} / {_healthManager.maxHP}";
        SpawnEnemyOnTakingDamage();
        if (IsDead())
            bossUI.SetActive(false);
    }

    protected override void OnDead()
    {
        DropChest();
        base.OnDead();
        GameSaving.instance.BossEndFight();
        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
    }

    protected override void PushBack(Vector2 dir) { }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        _healthStatsText.text = $"{_healthManager.hp} / {_healthManager.maxHP}";
    }

    protected override void Move()
    {
        if (!_canMove)
            return;

        base.Move();
    }

    protected virtual void SpawnEnemyOnTakingDamage()
    {
        GameObject enemy = Instantiate(spawnableEnemy, transform.position, Quaternion.identity);
        enemy.GetComponent<BaseEnemy>().ChangeToRandomDirection();
        enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * upForce, ForceMode2D.Impulse);
    }

    protected void StartFight()
    {
        _canMove = true;
    }

    protected void OnDestroy()
    {
        GameSaving.instance.OnBossStart -= StartFight;
    }

    protected virtual void DropChest()
    {
        Instantiate(chest, transform.position, Quaternion.identity);
    }
}

