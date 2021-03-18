using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Boss
{
    private bool isBlocking = false;
    [SerializeField] private float blockTime = 5f;
    public Collider2D block;
    public GameObject skeletonPrefab;
    public override void ApplyDamage(float damage, Vector2 dir)
    {
        if (Dead || isBlocking)
            return;
        base.ApplyDamage(damage, dir);
        SpawnSkeleton();

        int rand = UnityEngine.Random.Range(1, 3);
        if(rand == 1)
        {
            StartCoroutine(Block());
        }
    }

    private IEnumerator Block()
    {
        isBlocking = true;
        anim.SetBool("Block", true);
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(blockTime);
        anim.SetBool("Block", false);
        isBlocking = false;
        block.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void SpawnSkeleton()
    {
        GameObject skeleton = Instantiate(skeletonPrefab, transform.position, Quaternion.identity);
        int rand = UnityEngine.Random.Range(1, 3);
        Enemy skeletonEnemy = skeleton.GetComponent<Enemy>();
        int direction = skeletonEnemy.Direction == -1 ? 1 : -1;
        skeletonEnemy.Direction = direction;
        skeleton.GetComponent<Rigidbody2D>().AddForce(transform.up * 5f, ForceMode2D.Impulse);
    }


    protected override void Move()
    {
        if (isBlocking)
            return;
        base.Move();
    }
}
