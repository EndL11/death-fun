using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IDamagable
{
    void TakeDamage(float damage);
    void TakeDamage(float damage, Vector2 pushBack);
}

public interface IAttackable
{
    void Attack();
    void MakeAttack();
}

[System.Serializable]
public class HealthManager
{
    public float hp = 100f;
    public float maxHP = 100f;
    public Slider healthBar;
    [HideInInspector] public bool dead = false;

    public void Init()
    {
        healthBar.maxValue = maxHP;
        healthBar.value = hp;
    }

    public void UpdateHealth()
    {
        healthBar.value = hp;
    }

    public void ApplyDamage(float damage)
    {
        if (dead)
            return;

        hp -= damage;
        if (hp <= 0f)
        {
            dead = true;
            healthBar.gameObject.SetActive(false);
        }

        UpdateHealth();
    }

    public void AddHealth(float value)
    {
        hp += value;
        if (hp > maxHP)
            hp = maxHP;

        UpdateHealth();
    }

    public void AddMaxHP(float value)
    {
        maxHP += value;
        hp += value;

        healthBar.maxValue = maxHP;
        UpdateHealth();
    }
}



public abstract class Character : MonoBehaviour, IDamagable, IAttackable
{
    [Header("Health Managing")]
    [SerializeField] protected HealthManager _healthManager;
    protected Animator _anim;
    protected Rigidbody2D _rb;
    public ParticleSystem hurtParticles;
    public AudioSource hurtSFX;

    #region AttackSettings
    public float attackRange = 0.5f;
    public Transform attackPoint;
    public float damage;
    public LayerMask whatToAttack;
    #endregion

    public bool IsDead()
    {
        if (_healthManager != null)
            return _healthManager.dead;

        return false;
    }

    protected virtual void Start()
    {
        _healthManager.Init();
        _anim = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (_healthManager.dead)
            return;

        _healthManager.ApplyDamage(damage);
        hurtSFX.Play();

        if (_healthManager.dead)
            OnDead();
    }

    public virtual void TakeDamage(float damage, Vector2 pushBackDirection)
    {
        if (_healthManager.dead)
            return;

        _healthManager.ApplyDamage(damage);

        hurtSFX.Play();

        if (_healthManager.dead)
            OnDead();

        if (!_healthManager.dead)
        {
            if (hurtParticles != null)
                hurtParticles.Play();

            PushBack(pushBackDirection);

            StartCoroutine(HurtAnimation());
        }
    }

    protected virtual void PushBack(Vector2 dir)
    {
        //  reset velocity
        _rb.velocity = Vector2.zero;
        //  push player to direction
        _rb.AddForce(dir, ForceMode2D.Impulse);
    }

    protected IEnumerator HurtAnimation()
    {
        //  playing hurt animation
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255f, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(1, 1, 1, 1);
    }

    public virtual void Attack()
    {
        _anim.SetBool("Attack", false);
        _anim.SetBool("Attack", true);
    }

    public virtual void MakeAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatToAttack);
        Vector2 pushBack = transform.position.x < attackPoint.position.x ? Vector2.right : Vector2.left;
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
                continue;
            else if (collider.GetComponent<Character>().IsDead())
                continue;

            collider.GetComponent<IDamagable>().TakeDamage(damage, pushBack);
        }
        _anim.SetTrigger("AttackNull");
    }

    protected abstract void OnDead();
}

[System.Serializable]
public struct BlackHoleStats
{
    public float delay;
    public float damage;
    public float radius;
}


public class Player : Character
{

    #region SphereAttackSettings
    public BlackHoleStats blackHoleStats;
    private float _blackHoleDelay;
    public Transform blackHoleSpawnPoint;
    public GameObject blackHolePrefab;
    private Slider _blackholeDelaySlider;
    #endregion

    public Text healthStatsText;

    private PlayerMovement _playerMovement;

    public AudioSource attackSFX;
    public AudioSource spawnBlackHoleSFX;

    protected void Awake()
    {
        _healthManager.healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        healthStatsText = _healthManager.healthBar.GetComponentInChildren<Text>();
        _blackholeDelaySlider = GameObject.FindGameObjectWithTag("BlackHoleDelay").GetComponent<Slider>();
        _healthManager.Init();
    }

    public float maxHP()
    {
        return _healthManager.maxHP;
    }

    protected override void Start()
    {
        //  get animator component
        _anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        _rb = GetComponent<Rigidbody2D>();

        _playerMovement = GetComponent<PlayerMovement>();
        //  set standard sphere stats
        blackHoleStats.radius = blackHolePrefab.GetComponent<BlackHole>().radius;
        blackHoleStats.damage = blackHolePrefab.GetComponent<BlackHole>().damage;
        //  load saved player stats
        if (GameSaving.instance.playerStats.hp != 0 && PlayerPrefs.GetInt("@saved", 0) == 1 && !GameSaving.instance.IsTutorial())
        {
            damage = GameSaving.instance.playerStats.damage;
            _healthManager.hp = GameSaving.instance.playerStats.hp;
            _healthManager.maxHP = GameSaving.instance.playerStats.maxHp;

            blackHoleStats = GameSaving.instance.playerStats.blackHoleStats;
        }

        //  set blackhole delay on start game to 0
        _blackHoleDelay = 0f;
        //  setting sphere delay for slider
        _blackholeDelaySlider.maxValue = blackHoleStats.delay;
        _blackholeDelaySlider.value = blackHoleStats.delay - _blackHoleDelay;
        UpdateHealthText();
    }
    public override void TakeDamage(float damage, Vector2 pushBackDirection)
    {
        base.TakeDamage(damage, pushBackDirection);
        UpdateHealthText();
    }
    protected void Update()
    {
        if (!_playerMovement.CanMove)
            return;

        if (_blackHoleDelay > 0f)
        {
            _blackHoleDelay -= Time.deltaTime;
            _blackholeDelaySlider.value = blackHoleStats.delay - _blackHoleDelay;
        }
        else
        {
            //  if pressed '>' button
            if (Input.GetKeyDown(KeyCode.Period) && !IsDead())
            {
                SpawnBlackHole();
                _blackHoleDelay = blackHoleStats.delay;
            }
        }
    }

    public override void MakeAttack()
    {
        attackSFX.Play();
        base.MakeAttack();
    }

    private void UpdateHealthText()
    {
        healthStatsText.text = $"{_healthManager.hp} / {_healthManager.maxHP}";
    }

    private void SpawnBlackHole()
    {
        spawnBlackHoleSFX.Play();
        //  create gameobject based on 'blackHolePrefab'
        GameObject blackHole = Instantiate(blackHolePrefab, blackHoleSpawnPoint.position, transform.GetChild(0).rotation);
        BlackHole blackHoleInstance = blackHole.GetComponent<BlackHole>();
        blackHoleInstance.damage = blackHoleStats.damage;
        blackHoleInstance.radius = blackHoleStats.radius;
    }

    #region UpgradesMethods
    public void AddHealth(float value)
    {
        _healthManager.AddHealth(value);
        UpdateHealthText();
    }

    public void AddMaxHP(float value)
    {
        _healthManager.AddMaxHP(value);
        UpdateHealthText();
    }

    public void AddDamage(float value)
    {
        damage += value;
    }

    public void DecreaseSphereDelay(float value)
    {
        if (blackHoleStats.delay <= 2f)
            return;
        blackHoleStats.delay -= value;
        _blackholeDelaySlider.maxValue = blackHoleStats.delay;
    }

    public void IncreaseSphereDamage(float value)
    {
        blackHoleStats.damage += value;
    }

    public void IncreaseSphereRadius(float value)
    {
        blackHoleStats.radius += value;
    }
    #endregion
    public void SavePlayerStats()
    {
        //  save player stats
        PlayerPrefs.SetInt("@saved", 1);
        GameSaving.instance.playerStats.hp = _healthManager.hp;
        GameSaving.instance.playerStats.maxHp = _healthManager.maxHP;
        GameSaving.instance.playerStats.damage = damage;
        GameSaving.instance.playerStats.blackHoleStats = blackHoleStats;
        SaveToPlayerPrefs();
    }

    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("@hp", _healthManager.hp);
        PlayerPrefs.SetFloat("@maxhp", _healthManager.maxHP);
        PlayerPrefs.SetFloat("@damage", damage);
        PlayerPrefs.SetFloat("@spheredamage", blackHoleStats.damage);
        PlayerPrefs.SetFloat("@spheredelay", blackHoleStats.delay);
        PlayerPrefs.SetFloat("@sphereradius", blackHoleStats.radius);
    }

    protected override void OnDead()
    {
        //  hide health bar
        _healthManager.healthBar.gameObject.SetActive(false);
        //  reset layer from 'player' to default in order not to stop enemies
        gameObject.layer = 0;
        //  set rigidbody to static - not to fall player down
        _rb.bodyType = RigidbodyType2D.Static;
        //  set player to not solid object
        GetComponent<Collider2D>().isTrigger = true;

        if (PlayerPrefs.GetString("@mode") == "Hard Mode")
            PlayerPrefs.SetInt("@saved", 0);

        //  play die animation
        _anim.SetTrigger("Die");
    }
}
