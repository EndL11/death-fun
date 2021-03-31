using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    #region Player Stats
    [SerializeField] private float _hp = 100f;
    [SerializeField] private float _maxHP = 100f;
    [SerializeField] private float _damage = 15f;
    [SerializeField] private float _attackRange = 0.5f;
    //  sphere stats
    public float blackHoleDelay = 7f;
    private float _blackHoleDelay;
    private float _sphereDamage;
    private float _sphereRadius;
    #endregion
    [Space]
    //  gameobject to spawn (blackhole)
    [SerializeField] private GameObject blackHolePrefab;
    //  position for spawning black holes
    [SerializeField] private Transform spawnPosition;

    public LayerMask enemiesMask;

    private Animator _anim;
    private Rigidbody2D _rb;
    //  player UI
    private Slider _healthBar;
    private Text _healthBarHP;
    private Slider _blackholeDelaySlider;

    private PlayerMovement _playerMovement;

    public ParticleSystem hurtPatricles;

    private bool _dead = false;
    //  starting color (need for hurt animation)
    Color _color;

    public bool Dead
    {
        get { return _dead; }
    }

    public float MAXHP
    {
        get { return _maxHP; }
    }

    public float HP
    {
        get { return _hp; }
    }

    void Start()
    {
        _healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        _healthBarHP = _healthBar.GetComponentInChildren<Text>();
        _blackholeDelaySlider = GameObject.FindGameObjectWithTag("BlackHoleDelay").GetComponent<Slider>();
        //  get animator component
        _anim = GetComponentInChildren<Animator>();
        //  get rigidbody component
        _rb = GetComponent<Rigidbody2D>();

        _playerMovement = GetComponent<PlayerMovement>();
        //  set standard sphere stats
        _sphereRadius = blackHolePrefab.GetComponent<BlackHole>().radius;
        _sphereDamage = blackHolePrefab.GetComponent<BlackHole>().damage;
        //  load saved player stats
        if(GameSaving.instance.playerStats.hp != 0 && PlayerPrefs.GetInt("@saved", 0) == 1 && !GameSaving.instance.IsTutorial())
        {
            _damage = GameSaving.instance.playerStats.damage;
            _hp = GameSaving.instance.playerStats.hp;
            _maxHP = GameSaving.instance.playerStats.maxHp;

            blackHoleDelay = GameSaving.instance.playerStats.blackHoleDelay;
            _sphereDamage = GameSaving.instance.playerStats.blackHoleDamage;
            _sphereRadius = GameSaving.instance.playerStats.blackHoleRadius;
        }

        //  set healthbar start stats
        _healthBar.maxValue = _maxHP;
        _healthBar.value = _hp;

        _healthBarHP.text = $"{_hp} / {_maxHP}";
        //  get start color
        _color = GetComponentInChildren<SpriteRenderer>().material.color;
        //  set blackhole delay on start game to 0
        _blackHoleDelay = 0f;
        //  setting sphere delay for slider
        _blackholeDelaySlider.maxValue = blackHoleDelay;
        _blackholeDelaySlider.value = blackHoleDelay - _blackHoleDelay;
    }

    void Update()
    {
        if (!_playerMovement.CanMove)
            return;

        if (_blackHoleDelay > 0f)
        {
            _blackHoleDelay -= Time.deltaTime;
            _blackholeDelaySlider.value = blackHoleDelay - _blackHoleDelay;
        }
        else
        {
            //  if pressed right mouse button
            if (Input.GetKeyDown(KeyCode.Period) && !_dead)
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
        blackHole.GetComponent<BlackHole>().damage = _sphereDamage;
        blackHole.GetComponent<BlackHole>().radius = _sphereRadius;
    }

    public void ApplyAttack()
    {
        if (_dead)
            return;

        //  get all enemy object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition.position, _attackRange, enemiesMask);
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
                    enemy.GetComponent<AngrySkull>().ApplyDamage(_damage);
                    continue;
                }
                enemy.GetComponent<Witch>().ApplyDamage(_damage);
                continue;
            }
            //  damage each enemy
            enemy.GetComponent<Enemy>().ApplyDamage(_damage, directionToPush);
        }
    }

    public void ApplyDamage(float damage, Vector2 dir)
    {
        _hp -= damage;
		SoundMusicManager.instance.ApplyDamagePlayerPlay();
        //  update health bar
        _healthBar.value = _hp;
        _healthBarHP.text = $"{_hp} / {_maxHP}";
        if (_hp <= 0)
            DestroyObject();
        if (!_dead)
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
        _rb.velocity = Vector2.zero;
        //  push player to direction
        _rb.AddForce(dir, ForceMode2D.Impulse);
    }


    private IEnumerator HurtAnimation()
    {
        //  playing hurt animation
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = _color;
    }

    private void DestroyObject()
    {
        //  hide health bar
        _healthBar.gameObject.SetActive(false);
        _dead = true;
        //  reset layer from 'player' to default in order not to stop enemies
        gameObject.layer = 0;
        //  set rigidbody to static - not to fall player down
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        //  set player to not solid object
        GetComponent<Collider2D>().isTrigger = true;

        if(PlayerPrefs.GetString("@mode") == "Hard Mode")
            PlayerPrefs.SetInt("@saved", 0);

        //  play die animation
        _anim.SetTrigger("Die");
    }

    public void AddHealth(float value)
    {
        _hp += value;
        if (_hp > _maxHP)
            _hp = _maxHP;
        //  update healthbar 
        _healthBar.value = _hp;
        _healthBarHP.text = $"{_hp} / {_maxHP}";
    }

    public void AddMaxHP(float value)
    {
        _maxHP += value;
        _hp += value;
        //  update healthbar 
        _healthBar.maxValue = _maxHP;
        _healthBar.value = _hp;
        _healthBarHP.text = $"{_hp} / {_maxHP}";
    }

    public void AddDamage(float value)
    {
        _damage += value;
    }

    public void DecreaseSphereDelay(float value)
    {
        if (blackHoleDelay <= 2f)
            return;
        blackHoleDelay -= value;
        _blackholeDelaySlider.maxValue = blackHoleDelay;
    }

    public void IncreaseSphereDamage(float value)
    {
        _sphereDamage += value; 
    }

    public void IncreaseSphereRadius(float value)
    {
        _sphereRadius += value;
    }

    public void SavePlayerStats()
    {
        //  save player stats
        PlayerPrefs.SetInt("@saved", 1);
        GameSaving.instance.playerStats.hp = _hp;
        GameSaving.instance.playerStats.maxHp = _maxHP;
        GameSaving.instance.playerStats.damage = _damage;
        GameSaving.instance.playerStats.blackHoleDamage = _sphereDamage;
        GameSaving.instance.playerStats.blackHoleDelay = blackHoleDelay;
        GameSaving.instance.playerStats.blackHoleRadius = _sphereRadius;
        SaveToPlayerPrefs();
    }

    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("@hp", _hp);
        PlayerPrefs.SetFloat("@maxhp", _maxHP);
        PlayerPrefs.SetFloat("@damage", _damage);
        PlayerPrefs.SetFloat("@spheredamage", _sphereDamage);
        PlayerPrefs.SetFloat("@spheredelay", blackHoleDelay);
        PlayerPrefs.SetFloat("@sphereradius", _sphereRadius);
    }
}
