using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]  private float hp = 100f;
    [SerializeField]  private float maxHP = 100f;
    private float damage = 15f;
    //  gameobject to spawn (blackhole)
    [SerializeField] private GameObject blackHolePrefab;
    //  position for spawning black holes
    [SerializeField] private Transform spawnPosition;

    public LayerMask enemiesMask;

    private Animator anim;
    private Rigidbody2D rb;

    public Slider healthBar;

    private bool dead = false;
    Color c;

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
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
        c = GetComponentInChildren<SpriteRenderer>().material.color;

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
        Vector2 directionToPush = transform.position.x > spawnPosition.position.x ? Vector2.left : Vector2.right;
        foreach (var enemy in colliders)
        {
            enemy.GetComponent<Enemy>().ApplyDamage(damage, directionToPush);
        }
    }

    public void ApplyDamage(float damage, Vector2 dir)
    {
        PushBack(dir);
        hp -= damage;
        healthBar.value = hp;
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
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    private void DestroyObject()
    {
        healthBar.gameObject.SetActive(false);
        dead = true;
        gameObject.layer = 0;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().isTrigger = true;
        anim.SetTrigger("Die");
    }
}
