using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerStats
{
    public float damage;
    public float hp;
    public float maxHp;
    public BlackHoleStats blackHoleStats;
}

[System.Serializable]
public class EnemyAnalytics
{
    public enum Names { spider, snake, scorpion,
        zombie_1, zombie_2, zombie_3, zombie_4, knight_1, knight_2, knight_3,
        knight_4, ninja_1, ninja_2, ninja_3, ninja_4, ninja_5, skeleton, barbarian, witch, vampire, none };
    public GameObject prefab = null;
    public Names name;
    [HideInInspector] public bool show = false;
}

public class GameSaving : MonoBehaviour
{
    public static GameSaving instance;
    public event Action OnScoreChanged = () => { };
    public event Action OnEnemyDead = () => { };
    public event Action OnGameOver = () => { };
    public event Action OnBossStart = () => { };
    public event Action OnBossDie = () => { };
    public event Action OnEndLevel = () => { };
    public PlayerStats playerStats;
    public int score = 0;
    public int deadEnemies = 0;
    public int enemiesCount = 0;
    public List<GameObject> enemies = new List<GameObject>();
    private Dictionary<string, int> _enemiesDeadList = new Dictionary<string, int>();
    [SerializeField] private List<EnemyAnalytics> _analiticsPrefabs;

    [HideInInspector]
    public string[] _enemiesNames = System.Enum.GetNames(typeof(EnemyAnalytics.Names));

    public float difficultyCoefficient = 1f;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (SceneManager.GetActiveScene().name != "Tutorial")
                DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if(SceneManager.GetActiveScene().name != "Tutorial" && PlayerPrefs.GetInt("@complete", 0) == 1 && PlayerPrefs.GetInt("@level", 1) != 1)
        {
            LoadStats();
            PlayerPrefs.SetInt("@complete", 0);
        }
    }

    private void Start()
    {
        LoadDeadEnemies();        
    }

    public void AddScore(int value)
    {
        score += value;
        OnScoreChanged();
    }

    public void EnemyDead(GameObject enemy)
    {
        string name = enemy.GetComponent<BaseEnemy>().enemyName.ToString();
        bool deleted = enemies.Remove(enemy);
        if(deleted && name != "None")
                deadEnemies += 1;

        OnEnemyDead();

        if (deadEnemies == enemiesCount)
        {
            OnEndLevel();
        }
        //  do not save analytics on tutorial
        if (SceneManager.GetActiveScene().name == "Tutorial")
            return;

        if (_enemiesDeadList.ContainsKey(name))
        {
            _enemiesDeadList[name] += 1;
            return;
        }
        if(name != "")
            _enemiesDeadList.Add(name, 1);
    }

    public void GameOver()
    {
        foreach (var item in _analiticsPrefabs)
        {
            item.show = false;
        }

        OnGameOver();
        _enemiesDeadList.Clear();
    }

    public void SaveCompleteTutorial()
    {
        //  add saving history telling
        PlayerPrefs.SetInt("@tutor", 1);
    }

    public void SaveDeadEnemies()
    {
        foreach (var item in _enemiesDeadList)
        {
            if (item.Value > 0)
                PlayerPrefs.SetInt($"@{item.Key}", item.Value);
        }
    }

    private void LoadDeadEnemies()
    {
        foreach (string name in _enemiesNames)
        {
            _enemiesDeadList.Add(name, PlayerPrefs.GetInt($"@{name}", 0));
        }
    }

    public void ClearPlayerPrefs()
    {
        _enemiesDeadList.Clear();
        int tutorComplete = PlayerPrefs.GetInt("@tutor", 0);
        string mode = PlayerPrefs.GetString("@mode", "Normal Mode");
        int music = PlayerPrefs.GetInt("@music", 1);
        int sound = PlayerPrefs.GetInt("@sounds", 1);
        int history = PlayerPrefs.GetInt("@history", 0);
        float hardModeTime = PlayerPrefs.GetFloat("@awardHard", 0f);
        float normalModeTime = PlayerPrefs.GetFloat("@awardNormal", 0f);

        foreach (var item in _analiticsPrefabs)
        {
            item.show = false;
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("@tutor", tutorComplete);
        PlayerPrefs.SetInt("@music", music);
        PlayerPrefs.SetInt("@sounds", sound);
        PlayerPrefs.SetInt("@history", history);

        PlayerPrefs.SetFloat("@awardHard", hardModeTime);
        PlayerPrefs.SetFloat("@awardNormal", normalModeTime);

        PlayerPrefs.SetString("@mode", mode);
        LoadDeadEnemies();
    }

    private void SetDeadCountToPrefabs()
    {
        foreach (var item in _enemiesDeadList)
        {
            if (item.Value == 0)
                continue;

            EnemyAnalytics _tmp = _analiticsPrefabs.Find(x => x.name.ToString() == item.Key);
            if (_tmp?.prefab == null)
                return;
            GameObject prefab = _tmp.prefab;
            prefab.GetComponentInChildren<Text>().text = $"x{item.Value}";
            _tmp.show = true;
        }
    }

    public List<GameObject> GetAnalyticsObjects()
    {
        SetDeadCountToPrefabs();
        return _analiticsPrefabs.FindAll(x => x.show && x.prefab != null).ConvertAll(x => x.prefab);
    }

    public void Buy(int cost)
    {
        score -= cost;
        OnScoreChanged();
    }

    public void BossStartFight()
    {
        OnBossStart();
    }

    public void BossEndFight()
    {
        OnBossDie();
    }

    private void LoadStats()
    {
        playerStats.hp = PlayerPrefs.GetFloat("@hp", 0);
        playerStats.maxHp = PlayerPrefs.GetFloat("@maxhp", 0);
        playerStats.damage = PlayerPrefs.GetFloat("@damage", 0);
        playerStats.blackHoleStats.damage = PlayerPrefs.GetFloat("@spheredamage", 0);
        playerStats.blackHoleStats.delay = PlayerPrefs.GetFloat("@spheredelay", 0);
        playerStats.blackHoleStats.radius = PlayerPrefs.GetFloat("@sphereradius", 0);
    }

    public string ConvertGameTimeToString(float gameTime)
    {
        if (gameTime <= 0f)
            return "00:00:00";
        int timeValue = (int)gameTime;
        string time = "";
        int hours = timeValue / 3600;
        time += hours >= 10 ? $"{hours}:" : $"0{hours}:";
        timeValue -= hours * 3600;
        int minutes = timeValue / 60;
        time += minutes >= 10 ? $"{minutes}:" : $"0{minutes}:";
        timeValue -= minutes * 60;
        int seconds = timeValue;
        time += seconds > 0 ? seconds >= 10 ? $"{seconds}" : $"0{seconds}" : "00";
        return time;
    }

    public bool IsTutorial()
    {
        return SceneManager.GetActiveScene().name == "Tutorial";
    }
}
