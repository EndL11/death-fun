using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire : Boss
{
    [SerializeField] private float bloodLust = 100f;
    public ParticleSystem healingParticles;
    public GameObject vampirePrefab;

    [SerializeField] private float spawnDelay = 10f;
    private float _spawnDelay;

    protected override void Start()
    {
        base.Start();
        _spawnDelay = spawnDelay;
    }

    public override void Attack()
    {
        base.Attack();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlayerPoint.position, attackZone, whatIsPlayer);
        if(colliders.Length > 0)
        {
            hp += bloodLust;
            if (hp > maxHP)
                hp = maxHP;

            healthBar.value = hp;
            healthStats.text = $"{hp} / {maxHP}";
            healingParticles.Play();
        }
    }
    protected override void Update()
    {
        if (Dead)
            return;

        base.Update();

        if(_spawnDelay > 0f)
        {
            _spawnDelay -= Time.deltaTime;
        }
        else
        {
            if (canMove)
            {
                SpawnVampireClone();
                _spawnDelay = spawnDelay;
            }
        }
    }

    private void SpawnVampireClone()
    {
        GameObject vampire = Instantiate(vampirePrefab, transform.position, Quaternion.identity);
        Enemy vampireEnemy = vampire.GetComponent<Enemy>();
        int rand = UnityEngine.Random.Range(1, 3);
        int direction = vampireEnemy.Direction;
        if (rand == 1)
            direction = -1;
        else
            direction = 1;

        vampireEnemy.Direction = direction;
    }

}
