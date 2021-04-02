using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Witch : MonoBehaviour
{
    [SerializeField] private float _hp = 100f;
    [SerializeField] private float _maxHP = 100f;

    [SerializeField] private float _speed = 5f;

    public float spawnDelay = 3f;
    private float _spawnDelay;

    public GameObject bomberManPrefab;

    public Slider healthBar;

    private bool _dead = false;

    private Animator _anim;
    private Rigidbody2D _rb;

    public Transform leftPoint;
    public Transform rightPoint;

    [SerializeField] private int _direction = -1;

    private Color _color;

    public ParticleSystem hurtParticles;

    private bool _isSpawning = false;

    public EnemyAnalytics.Names enemyName;

    public bool Dead
    {
        get { return _dead; }
    }

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        healthBar.maxValue = _maxHP;
        healthBar.value = _hp;

        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction < 0 ? 0f : 180f), 0f);
        _spawnDelay = spawnDelay;

        _color = GetComponentInChildren<SpriteRenderer>().material.color;
    }

    private void Update()
    {
        if (_dead)
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

        if (!_dead && !_isSpawning)
            Move();
    }

    public void ApplyDamage(float damage)
    {
        if (_dead)
            return;

        _hp -= damage;
        healthBar.value = _hp;
        if (_hp <= 0f)
            DestroyWitch();
        if (!_dead)
        {
            StartCoroutine(HurtAnimation());
            hurtParticles.Play();
        }
    }

    private void DestroyWitch()
    {
        _anim.SetBool("Fly", false);
        healthBar.gameObject.SetActive(false);
        _dead = true;
        _anim.SetTrigger("Die");
        _rb.bodyType = RigidbodyType2D.Dynamic;
        GameSaving.instance.EnemyDead(gameObject);
    }

    private void Move()
    {
        _anim.SetBool("Fly", true);
        transform.Translate(transform.right * _direction * Time.deltaTime * _speed);
        if ((_direction == 1 && transform.position.x > rightPoint.position.x) || (_direction == -1 && transform.position.x < leftPoint.position.x))
        {
            ChangeMovementDirection();
        }
    }

    private IEnumerator SpawnRandomEnemy()
    {
        _isSpawning = true;
        _anim.SetBool("Fly", false);
        yield return new WaitForSeconds(0.3f);
        GameObject enemy = Instantiate(bomberManPrefab, transform.position, Quaternion.identity);
        int rand = UnityEngine.Random.Range(1, 3);
        BomberMan bomber = enemy.GetComponent<BomberMan>();
        int direction = bomber.Direction == -1 ? 1 : -1;
        bomber.Direction = direction;
        _isSpawning = false;
    }

    private void ChangeMovementDirection()
    {
        //  set direction to another
        _direction = _direction == 1 ? -1 : 1;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (_direction < 0 ? 0f : 180f), 0f);
    }

    private IEnumerator HurtAnimation()
    {
        // set sprite color to red         
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        //  wait 0.2 seconds
        yield return new WaitForSeconds(0.2f);
        //  set start color
        GetComponentInChildren<SpriteRenderer>().material.color = _color;
    }
}
