using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float attackRange = 0.5f;
    //  sphere stats
    public float blackHoleDelay = 7f;
    private float _blackHoleDelay;
    private float sphereDamage;
    private float sphereRadius;
    [Space]
    //  gameobject to spawn (blackhole)
    [SerializeField] private GameObject blackHolePrefab;
    //  position for spawning black holes
    [SerializeField] private Transform spawnPosition;

    public LayerMask enemiesMask;

    private Animator anim;
    private Rigidbody2D rb;
    //  player UI
    private Slider healthBar;
    private Text healthBarHP;
    private Slider blackholeDelaySlider;

    private PlayerMovement playerMovement;

    public ParticleSystem hurtPatricles;

    private bool dead = false;
    //  starting color (need for hurt animation)
    Color c;

    public bool Dead
    {
        get { return dead; }
    }

    public float MAXHP
    {
        get { return maxHP; }
    }

    public float HP
    {
        get { return hp; }
    }

    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        healthBarHP = healthBar.GetComponentInChildren<Text>();
        blackholeDelaySlider = GameObject.FindGameObjectWithTag("BlackHoleDelay").GetComponent<Slider>();
        //  get animator component
        anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        rb = GetComponent<Rigidbody2D>();

        playerMovement = GetComponent<PlayerMovement>();
        //  set standard sphere stats
        sphereRadius = blackHolePrefab.GetComponent<BlackHole>().radius;
        sphereDamage = blackHolePrefab.GetComponent<BlackHole>().damage;
        //  load saved player stats
        if(GameSaving.instance.playerStats.hp != 0 && PlayerPrefs.GetInt("@saved", 0) == 1 && !GameSaving.instance.IsTutorial())
        {
            damage = GameSaving.instance.playerStats.damage;
            hp = GameSaving.instance.playerStats.hp;
            maxHP = GameSaving.instance.playerStats.maxHp;

            blackHoleDelay = GameSaving.instance.playerStats.blackHoleDelay;
            sphereDamage = GameSaving.instance.playerStats.blackHoleDamage;
            sphereRadius = GameSaving.instance.playerStats.blackHoleRadius;
        }

        //  set healthbar start stats
        healthBar.maxValue = maxHP;
        healthBar.value = hp;

        healthBarHP.text = $"{hp} / {maxHP}";
        //  get start color
        c = GetComponentInChildren<SpriteRenderer>().material.color;
        //  set blackhole delay on start game to 0
        _blackHoleDelay = 0f;
        //  setting sphere delay for slider
        blackholeDelaySlider.maxValue = blackHoleDelay;
        blackholeDelaySlider.value = blackHoleDelay - _blackHoleDelay;
    }

    void Update()
    {
        if (!playerMovement.CanMove)
            return;

        if (_blackHoleDelay > 0f)
        {
            _blackHoleDelay -= Time.deltaTime;
            blackholeDelaySlider.value = blackHoleDelay - _blackHoleDelay;
        }
        else
        {
            //  if pressed right mouse button
            if (Input.GetKeyDown(KeyCode.Period) && !dead)
            {
                SpawnBlackHole();
                _blackHoleDelay = blackHoleDelay;
            }
        }

    }

    private void SpawnBlackHole()
    {
        SoundMusicManager.instance.SpawnBlackHolePlay();
		//  create gameobject based on 'blackHolePrefab'
        GameObject blackHole = Instantiate(blackHolePrefab, spawnPosition.position, transform.GetChild(0).rotation);
        blackHole.GetComponent<BlackHole>().damage = sphereDamage;
        blackHole.GetComponent<BlackHole>().radius = sphereRadius;
    }

    public void ApplyAttack()
    {
        if (dead)
            return;

        //  get all enemy object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition.position, attackRange, enemiesMask);
        //  calculating push direction
        Vector2 directionToPush = transform.position.x > spawnPosition.position.x ? Vector2.left : Vector2.right;
		SoundMusicManager.instance.WooahPlay();
        foreach (var enemy in colliders)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if(enemyScript == null)
            {
                Witch witch = enemy.GetComponent<Witch>();
                if(witch == null)
                {
                    enemy.GetComponent<AngrySkull>().ApplyDamage(damage);
                    continue;
                }
                enemy.GetComponent<Witch>().ApplyDamage(damage);
                continue;
            }
            //  damage each enemy
            enemy.GetComponent<Enemy>().ApplyDamage(damage, directionToPush);
        }
    }

    public void ApplyDamage(float damage, Vector2 dir)
    {
        hp -= damage;
		SoundMusicManager.instance.ApplyDamagePlayerPlay();
        //  update health bar
        healthBar.value = hp;
        healthBarHP.text = $"{hp} / {maxHP}";
        if (hp <= 0)
            DestroyObject();
        if (!dead)
        {
            hurtPatricles.Play();
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

        if(PlayerPrefs.GetString("@mode") == "Hard Mode")
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
        healthBarHP.text = $"{hp} / {maxHP}";
    }

    public void AddMaxHP(float value)
    {
        maxHP += value;
        hp += value;
        //  update healthbar 
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
        healthBarHP.text = $"{hp} / {maxHP}";
    }

    public void AddDamage(float value)
    {
        damage += value;
    }

    public void DecreaseSphereDelay(float value)
    {
        if (blackHoleDelay <= 2f)
            return;
        blackHoleDelay -= value;
        blackholeDelaySlider.maxValue = blackHoleDelay;
    }

    public void IncreaseSphereDamage(float value)
    {
        sphereDamage += value; 
    }

    public void IncreaseSphereRadius(float value)
    {
        sphereRadius += value;
    }

    public void SavePlayerStats()
    {
        //  save player stats
        PlayerPrefs.SetInt("@saved", 1);
        GameSaving.instance.playerStats.hp = hp;
        GameSaving.instance.playerStats.maxHp = maxHP;
        GameSaving.instance.playerStats.damage = damage;
        GameSaving.instance.playerStats.blackHoleDamage = sphereDamage;
        GameSaving.instance.playerStats.blackHoleDelay = blackHoleDelay;
        GameSaving.instance.playerStats.blackHoleRadius = sphereRadius;
        SaveToPlayerPrefs();
    }

    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("@hp", hp);
        PlayerPrefs.SetFloat("@maxhp", maxHP);
        PlayerPrefs.SetFloat("@damage", damage);
        PlayerPrefs.SetFloat("@spheredamage", sphereDamage);
        PlayerPrefs.SetFloat("@spheredelay", blackHoleDelay);
        PlayerPrefs.SetFloat("@sphereradius", sphereRadius);
    }
}
