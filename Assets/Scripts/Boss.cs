using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    [SerializeField] protected GameObject healthBarObject;
    protected Text healthStats;
    private bool canMove = false;
    protected void Awake()
    {
        healthBar = healthBarObject.GetComponentInChildren<Slider>();
        healthStats = healthBar.GetComponentInChildren<Text>();
        healthBarObject.SetActive(false);
        healthStats.text = $"{hp} / {maxHP}";

        //  for testing
        //healthBarObject.SetActive(true);
    }

    protected override void Start()
    {
        base.Start();
        GameSaving.instance.OnBossStart += StartFight;        
    }

    protected override void PushBack(Vector2 dir)
    {

    }

    protected override void Move()
    {
        if (!canMove)
            return;

        base.Move();
    }

    public override void ApplyDamage(float damage, Vector2 dir)
    {
        if (Dead)
            return;

        SoundMusicManager.instance.DamageBossSoundPlay();
        base.ApplyDamage(damage, dir);
        healthBar.value = hp;
        healthStats.text = $"{hp} / {maxHP}";
        if (hp <= 0f)
        {
            healthBarObject.SetActive(false);
        }
    }

    private void StartFight()
    {
        canMove = true;
    }

    private void OnDestroy()
    {
        GameSaving.instance.OnBossStart -= StartFight;
    }
} 
