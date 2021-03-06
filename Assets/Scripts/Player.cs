using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float hp = 100f;
    private float damage = 15f;
    //  gameobject to spawn (blackhole)
    [SerializeField] private GameObject blackHolePrefab;
    //  position for spawning black holes
    [SerializeField] private Transform spawnPosition;

    public LayerMask enemiesMask;

    private Animator anim;
    private Rigidbody2D rb;

    private bool dead = false;

    public bool Dead
    {
        get { return dead; }
    }

    void Start()
    {
        //  get animator component
        anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))        //  if pressed right mouse button
            SpawnBlackHole();
    }

    private void SpawnBlackHole()
    {
        Instantiate(blackHolePrefab, spawnPosition.position, transform.rotation);
    }

    public void ApplyAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCapsuleAll(spawnPosition.position, new Vector2(0.4f, .5f), CapsuleDirection2D.Vertical, 0f, enemiesMask);
        Vector2 directionToPush = new Vector2(( transform.position.x > spawnPosition.position.x ? transform.position.x - 3f : transform.position.x + 3f), transform.position.y + 3f);
        foreach (var enemy in colliders)
        {
            enemy.GetComponent<Enemy>().ApplyDamage(damage, directionToPush);
        }
    }

    public void ApplyDamage(float damage, Vector2 dir)
    {
        PushBack(dir);
        hp -= damage;
        if (hp <= 0)
            DestroyObject();
    }

    private void PushBack(Vector2 dir)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir, ForceMode2D.Impulse);
        StartCoroutine(HurtAnimation());
    }


    private IEnumerator HurtAnimation()
    {
        Color c = GetComponentInChildren<SpriteRenderer>().material.color;
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    private void DestroyObject()
    {
        dead = true;
        gameObject.layer = 0;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().isTrigger = true;
        anim.SetTrigger("Die");
    }
}
