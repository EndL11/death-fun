using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Witch : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHP = 100f;

    [SerializeField] private float speed = 5f;

    public float spawnDelay = 3f;
    private float _spawnDelay;

    [SerializeField] private GameObject bomberManPrefab;

    [SerializeField] private Slider healthBar;

    private bool dead = false;

    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private int direction = -1;

    private Color c;

    [SerializeField] private ParticleSystem hurtParticles;

    private bool isSpawning = false;

    public bool Dead
    {
        get { return dead; }
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthBar.maxValue = maxHP;
        healthBar.value = hp;

        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
        _spawnDelay = spawnDelay;

        c = GetComponentInChildren<SpriteRenderer>().material.color;
    }

    private void Update()
    {
        if (dead)
            return;

        if (_spawnDelay > 0f)
        {
            _spawnDelay -= Time.deltaTime;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(SpawnRandomEnemy());
            _spawnDelay = spawnDelay;
        }

        if (!dead && !isSpawning)
            Move();
    }

    public void ApplyDamage(float damage)
    {
        if (dead)
            return;

        hp -= damage;
        healthBar.value = hp;
        if (hp <= 0f)
            DestroyWitch();
        if (!dead)
        {
            StartCoroutine(HurtAnimation());
            hurtParticles.Play();
        }
    }

    private void DestroyWitch()
    {
        anim.SetBool("Fly", false);
        healthBar.gameObject.SetActive(false);
        dead = true;
        anim.SetTrigger("Die");
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Move()
    {
        anim.SetBool("Fly", true);
        transform.Translate(transform.right * direction * Time.deltaTime * speed);
        if ((direction == 1 && transform.position.x > rightPoint.position.x) || (direction == -1 && transform.position.x < leftPoint.position.x))
        {
            ChangeMovementDirection();
        }
    }

    private IEnumerator SpawnRandomEnemy()
    {
        isSpawning = true;
        anim.SetBool("Fly", false);
        yield return new WaitForSeconds(0.3f);
        GameObject enemy = Instantiate(bomberManPrefab, transform.position, Quaternion.identity);
        int rand = UnityEngine.Random.Range(1, 3);
        BomberMan bomber = enemy.GetComponent<BomberMan>();
        int direction = bomber.Direction == -1 ? 1 : -1;
        bomber.Direction = direction;
        isSpawning = false;
    }

    private void ChangeMovementDirection()
    {
        //  set direction to another
        direction = direction == 1 ? -1 : 1;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
    }

    private IEnumerator HurtAnimation()
    {
        // set sprite color to red         
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        //  wait 0.2 seconds
        yield return new WaitForSeconds(0.2f);
        //  set start color
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }
}
