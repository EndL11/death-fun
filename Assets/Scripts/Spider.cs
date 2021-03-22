using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Boss
{
    public GameObject miniSpider;
    public override void ApplyDamage(float damage, Vector2 dir)
    {
        if (Dead)
            return;
        base.ApplyDamage(damage, dir);
        SpawnMiniSpider();
    }

    private void SpawnMiniSpider()
    {
        GameObject spider = Instantiate(miniSpider, transform.position, Quaternion.identity);
        int rand = UnityEngine.Random.Range(1, 3);
        Enemy spiderEnemy = spider.GetComponent<Enemy>();
        int direction = spiderEnemy.Direction;
        if (rand == 1)
            direction = -1;
        else
            direction = 1;

        spiderEnemy.Direction = direction;
        spider.GetComponent<Rigidbody2D>().AddForce(transform.up * 5f, ForceMode2D.Impulse);
    }

}
