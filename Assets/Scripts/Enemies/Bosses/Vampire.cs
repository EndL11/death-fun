using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire : Boss
{
    public float bloodLust = 100f;
    public float spawnDelay = 10f;
    private float _spawnDelay;
    public ParticleSystem healingParticles;

    protected override void Start()
    {
        base.Start();
        _spawnDelay = spawnDelay;
    }

    public override void MakeAttack()
    {
        base.MakeAttack();
        TryToBloodLust();
    }

    protected override void Update()
    {
        if (!_canMove || IsDead())
            return;
            
        base.Update();

        if (_spawnDelay > 0f)
        {
            _spawnDelay -= Time.deltaTime;
        }
        else
        {
            if (_canMove)
            {
                _spawnDelay = spawnDelay;
                SpawnVampireClone();
            }
        }
    }

    protected override void SpawnEnemyOnTakingDamage() { }

    private void SpawnVampireClone()
    {
        GameObject vampire = Instantiate(spawnableEnemy, transform.position, Quaternion.identity);
        vampire.GetComponent<BaseEnemy>().ChangeToRandomDirection();
    }

    private void TryToBloodLust()
    {
        Collider2D collider = Physics2D.OverlapCircle(checkPlayerPoint.position, attackRange, whatToAttack);
        if (collider)
        {
            _healthManager.AddHealth(bloodLust);
            _healthStatsText.text = $"{_healthManager.hp} / {_healthManager.maxHP}";
            healingParticles.Play();
        }
    }
}