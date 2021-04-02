using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Boss
{
    private bool _isBlocking = false;
    [SerializeField] private float _blockTime = 5f;
    public Collider2D block;
    public GameObject skeletonPrefab;
    public override void ApplyDamage(float damage, Vector2 dir)
    {
        if (Dead || _isBlocking)
            return;
        base.ApplyDamage(damage, dir);
        SpawnSkeleton();

        int rand = UnityEngine.Random.Range(1, 6);
        if(rand == 1)
        {
            StartCoroutine(Block());
        }
    }

    private IEnumerator Block()
    {
        _isBlocking = true;
        _anim.SetBool("Block", true);
        _rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(_blockTime);
        _anim.SetBool("Block", false);
        _isBlocking = false;
        block.enabled = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void SpawnSkeleton()
    {
        GameObject skeleton = Instantiate(skeletonPrefab, transform.position, Quaternion.identity);
        int rand = UnityEngine.Random.Range(1, 3);
        Enemy skeletonEnemy = skeleton.GetComponent<Enemy>();
        int direction = skeletonEnemy.Direction;
        if (rand == 1)
            direction = -1;
        else
            direction = 1;

        skeletonEnemy.Direction = direction;
        skeleton.GetComponent<Rigidbody2D>().AddForce(transform.up * 5f, ForceMode2D.Impulse);
    }


    protected override void Move()
    {
        if (_isBlocking)
            return;
        base.Move();
    }
}
