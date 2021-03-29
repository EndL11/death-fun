using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngrySkull : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHP = 100f;

    public float spawnEnemyDelay = 10f;
    private float _spawnDelay;
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private float pushForce = 10f;
    private bool dead = false;
    [SerializeField] private int direction = -1;
    [SerializeField] private float speed = 5f;

    public LayerMask whatIsGround;

    public Transform checkPlatformEndPoint;

    private Animator anim;
    private Color c;

    public ParticleSystem hurtParticles;
    public GameObject soulPrefab;

    public GameObject bossUI;
    private Slider healthBar;
    private Text healthStats;

    public GameObject chest;

    public EnemyAnalytics.Names _name;

    public float HP
    {
        get { return hp; }
    }

    public float MaxHP
    {
        get { return maxHP; }
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        _spawnDelay = spawnEnemyDelay;
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
        healthBar = bossUI.GetComponentInChildren<Slider>();
        healthStats = bossUI.transform.GetChild(1).GetComponentInChildren<Text>();

        c = GetComponentInChildren<SpriteRenderer>().material.color;

        healthBar.maxValue += maxHP;
        healthBar.value += hp;

        healthStats.text = $"{healthBar.value} / {healthBar.maxValue}";

        bossUI.SetActive(true);
    }

    void Update()
    {
        if (dead)
            return;

        if(_spawnDelay > 0f)
        {
            _spawnDelay -= Time.deltaTime;
        }
        else
        {
            StartCoroutine(SpawnEnemies());
            _spawnDelay = spawnEnemyDelay;
        }

        if (!isEndPlatform() && isGrounded())
            Move();
        else if(isGrounded() && isEndPlatform())
            ChangeMovementDirection();
    }

    private IEnumerator SpawnEnemies()
    {
        anim.SetTrigger("Spawn");
        yield return new WaitForSeconds(.3f);
        Vector2 leftSpawnPoint = new Vector2(transform.position.x - 0.3f, transform.position.y);
        Vector2 rightSpawnPoint = new Vector2(transform.position.x + 0.3f, transform.position.y);
        Vector2 pushDirection = new Vector2(1f, .3f);

        GameObject leftEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], leftSpawnPoint, Quaternion.identity);
        leftEnemy.GetComponent<Rigidbody2D>().AddForce(-pushDirection * pushForce, ForceMode2D.Impulse);

        GameObject rightEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], rightSpawnPoint, Quaternion.identity);
        rightEnemy.GetComponent<Rigidbody2D>().AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

        GameObject centerEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
        centerEnemy.GetComponent<Rigidbody2D>().AddForce(transform.up * pushForce, ForceMode2D.Impulse);

        leftEnemy = RandomDirection(leftEnemy);
        centerEnemy = RandomDirection(centerEnemy);
        rightEnemy = RandomDirection(rightEnemy);
    }

    private GameObject RandomDirection(GameObject enemy)
    {
        int rand = Random.Range(1, 3);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        int direction = enemyScript.Direction;
        if (rand == 1)
            direction = -1;
        else
            direction = 1;

        enemyScript.Direction = direction;
        return enemy;
    }

    private void Move()
    {
        transform.Translate(transform.right * direction * speed * Time.deltaTime);
    }

    protected void ChangeMovementDirection()
    {
        //  set direction to another
        direction = direction == 1 ? -1 : 1;
        //  rotate sprite according to direction
        transform.GetChild(0).rotation = Quaternion.Euler(0f, (direction < 0 ? 0f : 180f), 0f);
    }

    protected bool isEndPlatform()
    {
        //  return true if platform is ended
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPlatformEndPoint.position, 0.3f, whatIsGround);
        return colliders.Length == 0;
    }

    protected bool isGrounded()
    {
        //  return true if enemy is on ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, whatIsGround);
        return colliders.Length > 0;
    }

    public void ApplyDamage(float damage)
    {
        hp -= damage;
        SoundMusicManager.instance.PunchPlay();
        if (hp - damage <= 0f)
            healthBar.value -= hp;
        else
            healthBar.value -= damage;

        healthStats.text = $"{healthBar.value} / {healthBar.maxValue}";

        if (hp <= 0f)
            DestroySkull();

        if (!dead)
        {
            //  show particles
            hurtParticles.Play();
            //  play hurt animation
            StartCoroutine(HurtAnimation());
        }
    }

    private void DestroySkull()
    {
        dead = true;
        SpawnSoul();
        healthBar.maxValue -= maxHP;
        healthStats.text = $"{healthBar.value} / {healthBar.maxValue}";
        if (chest != null)
            Instantiate(chest, transform.position, Quaternion.identity);

        if (healthBar.value == 0)
            bossUI.SetActive(false);

        GameSaving.instance.EnemyDead(gameObject);

        Destroy(gameObject);
    }

    private void SpawnSoul()
    {
        GameObject soul = Instantiate(soulPrefab, transform.position, Quaternion.identity);
        Destroy(soul, 1.5f);
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
