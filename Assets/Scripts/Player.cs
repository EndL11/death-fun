using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float damage = 15f;
    public float blackHoleDelay = 7f;
    private float _blackHoleDelay;
    //  gameobject to spawn (blackhole)
    [SerializeField] private GameObject blackHolePrefab;
    //  position for spawning black holes
    [SerializeField] private Transform spawnPosition;

    [SerializeField] private float attackRange = 0.5f;

    public LayerMask enemiesMask;

    private Animator anim;
    private Rigidbody2D rb;

    private Slider healthBar;
    private Slider blackholeDelaySlider;

    private bool dead = false;
    //  starting color (need for hunt animation)
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
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        blackholeDelaySlider = GameObject.FindGameObjectWithTag("BlackHoleDelay").GetComponent<Slider>();
        //  get animator component
        anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        rb = GetComponent<Rigidbody2D>();
        //  load saved player stats
        if(GameSaving.instance != null && GameSaving.instance?.playerStats.damage != 0f && PlayerPrefs.GetInt("@saved", 0) == 1)
        {
            damage = GameSaving.instance.playerStats.damage;
            hp = GameSaving.instance.playerStats.hp;
            maxHP = GameSaving.instance.playerStats.maxHp;
        }
        //  set healthbar start stats
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
        //  get start color
        c = GetComponentInChildren<SpriteRenderer>().material.color;
        //  set blackhole delay on start game to 0
        _blackHoleDelay = 0f;
        blackholeDelaySlider.maxValue = blackHoleDelay;
        blackholeDelaySlider.value = blackHoleDelay - _blackHoleDelay;
    }

    void Update()
    {
        if (!GetComponent<PlayerMovement>().CanMove)
            return;

        if (_blackHoleDelay > 0f)
        {
            _blackHoleDelay -= Time.deltaTime;
            blackholeDelaySlider.value = blackHoleDelay - _blackHoleDelay;
        }
        else
        {
            //  if pressed right mouse button
            if (Input.GetMouseButtonDown(1) && !dead)
            {
                SpawnBlackHole();
                _blackHoleDelay = blackHoleDelay;
            }
        }

    }

    private void SpawnBlackHole()
    {
        //  create gameobject based on 'blackHolePrefab'
        Instantiate(blackHolePrefab, spawnPosition.position, transform.GetChild(0).rotation);
    }

    public void ApplyAttack()
    {
        if (dead)
            return;

        //  get all enemy object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition.position, attackRange, enemiesMask);
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
        if (SceneManager.GetActiveScene().buildIndex != 1)
            PlayerPrefs.SetInt("@coins", GameSaving.instance.score);

        PlayerPrefs.SetInt("@saved", 0);
        //  play die animation
        anim.SetTrigger("Die");
    }

    public void AddHealth(float value)
    {
        hp += value;
        if (hp > maxHP)
            hp = maxHP;
        //  update healthbar 
        healthBar.value = hp;
    }

    public void AddMaxHP(float value)
    {
        maxHP += value;
        hp += value;
        //  update healthbar 
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
    }

    public void AddDamage(float value)
    {
        damage += value;
    }

    public void SavePlayerStats()
    {
        //  if it's tutorial level not to save player stats
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            //  saving tutor complete
            GameSaving.instance.SaveCompleteTutorial();
            return;
        }

        PlayerPrefs.SetInt("@coins", GameSaving.instance.score);

        //  save player stats
        PlayerPrefs.SetInt("@saved", 1);
        GameSaving.instance.playerStats.hp = hp;
        GameSaving.instance.playerStats.maxHp = maxHP;
        GameSaving.instance.playerStats.damage = damage;
    }
}
