using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    [SerializeField] protected GameObject _healthBarObject;
    protected Text _healthStats;
    protected bool _canMove = false;
    [SerializeField] private GameObject _chest;

    protected void Awake()
    {
        healthBar = _healthBarObject.GetComponentInChildren<Slider>();
        _healthStats = healthBar.GetComponentInChildren<Text>();
        _healthBarObject.SetActive(false);
        _healthStats.text = $"{_hp} / {_maxHP}";
    }

    protected override void Start()
    {
        base.Start();
        GameSaving.instance.OnBossStart += StartFight;        
    }

    protected override void PushBack(Vector2 dir) { }

    protected override void Move()
    {
        if (!_canMove)
            return;

        base.Move();
    }

    public override void ApplyDamage(float damage, Vector2 dir)
    {
        if (Dead)
            return;
        base.ApplyDamage(damage, dir);
        healthBar.value = _hp;
        _healthStats.text = $"{_hp} / {_maxHP}";
        if (_hp <= 0f)
        {
            _healthBarObject.SetActive(false);
        }
    }

    private void StartFight()
    {
        _canMove = true;
    }

    private void OnDestroy()
    {
        GameSaving.instance.OnBossStart -= StartFight;
    }

    protected override void DestroyEnemy()
    {
        if(_chest != null)
            Instantiate(_chest, transform.position, Quaternion.identity);
        base.DestroyEnemy();
        GameSaving.instance.BossEndFight();
    }
} 
