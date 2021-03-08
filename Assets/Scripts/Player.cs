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

    public float MAXHP
    {
        get { return maxHP; }
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
        //  create gameobject based on 'blackHolePrefab'
        Instantiate(blackHolePrefab, spawnPosition.position, transform.GetChild(0).rotation);
    }

    public void ApplyAttack()
    {
        //  get all enemy object
        Collider2D[] colliders = Physics2D.OverlapCapsuleAll(spawnPosition.position, new Vector2(0.4f, .5f), CapsuleDirection2D.Vertical, 0f, enemiesMask);
        //  calculating push direction
        Vector2 directionToPush = transform.position.x > spawnPosition.position.x ? Vector2.left : Vector2.right;
        foreach (var enemy in colliders)
        {
            //  damage each enemy
            enemy.GetComponent<Enemy>().ApplyDamage(damage, directionToPush);
        }
    }

    public void ApplyDamage(float damage, Vector2 dir)
    {
        hp -= damage;
        //  update health bar
        healthBar.value = hp;
        if (hp <= 0)
            DestroyObject();
        if (!dead)
        {
            //  push player back
            PushBack(dir);
            //  play hurt animation
            StartCoroutine(HurtAnimation());
        }
    }

    private void PushBack(Vector2 dir)
    {
        //  reset velocity
        rb.velocity = Vector2.zero;
        //  push player to direction
        rb.AddForce(dir, ForceMode2D.Impulse);        
    }


    private IEnumerator HurtAnimation()
    {
        //  playing hurt animation
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    private void DestroyObject()
    {
        //  hide health bar
        healthBar.gameObject.SetActive(false);
        dead = true;
        //  reset layer from 'player' to default in order not to stop enemies
        gameObject.layer = 0;
        //  set rigidbody to static - not to fall player down
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        //  set player to not solid object
        GetComponent<Collider2D>().isTrigger = true;
        //  play die animation
        anim.SetTrigger("Die");
    }

    public void AddHealth(float value)
    {
        hp += value;
        if (hp > maxHP)
            hp = maxHP;

        healthBar.value = hp;
    }
}
