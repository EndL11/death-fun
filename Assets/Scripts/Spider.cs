using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
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
        int rand = UnityEngine.Random.Range(1, 3);
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        if(rand == 1)
        {
            rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        GameObject spider = Instantiate(miniSpider, transform.position, rotation);
        spider.GetComponent<Rigidbody2D>().AddForce(transform.up * 2f, ForceMode2D.Impulse);
    }

    protected override void PushBack(Vector2 dir)
    {

    }
}
