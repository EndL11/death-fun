using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public struct PlayerStats
{
    public float damage;
    public float hp;
    public float maxHp;
    public float blackHoleDelay;
    public float blackHoleDamage;
    public float blackHoleRadius;
}

[System.Serializable]
public class EnemyAnalytics
{
    public enum Names { spider, snake, scorpion,
        zombie_1, zombie_2, zombie_3, zombie_4, knight_1, knight_2, knight_3,
        knight_4, ninja_1, ninja_2, ninja_3, ninja_4, ninja_5, skeleton, barbarian, witch, bomberMan };
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
    public PlayerStats playerStats;
    public int score = 0;
    public int deadEnemies = 0;
    private Dictionary<string, int> enemiesDeadList = new Dictionary<string, int>();
    [SerializeField] private List<EnemyAnalytics> analiticsPrefabs;

    [HideInInspector]
    public string[] ENEMIES = System.Enum.GetNames(typeof(EnemyAnalytics.Names));
    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
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


    public void EnemyDead(string name)
    {
        deadEnemies += 1;
        OnEnemyDead();

        if (SceneManager.GetActiveScene().buildIndex == 1)
            return;

        if (enemiesDeadList.ContainsKey(name))
        {
            enemiesDeadList[name] += 1;
            return;
        }
        enemiesDeadList.Add(name, 1);
    }

    public void GameOver()
    {
        OnGameOver();
    }

    public void SaveCompleteTutorial()
    {
        PlayerPrefs.SetInt("@tutor", 1);
    }

    public void SaveDeadEnemies()
    {
        foreach (var item in enemiesDeadList)
        {
            if (item.Value > 0)
                PlayerPrefs.SetInt($"@{item.Key}", item.Value);
        }
    }

    private void LoadDeadEnemies()
    {
        foreach (string name in ENEMIES)
        {
            enemiesDeadList.Add(name, PlayerPrefs.GetInt($"@{name}", 0));
        }
    }

    public void ClearPlayerPrefs()
    {
        int tutorComplete = PlayerPrefs.GetInt("@tutor", 0);
        enemiesDeadList.Clear();
        int _score = PlayerPrefs.GetInt("@coins", 0);

        string mode = PlayerPrefs.GetString("@mode", "Normal Mode");

        int music = PlayerPrefs.GetInt("@music", 1);
        int sound = PlayerPrefs.GetInt("@sounds", 1);

        foreach (var item in analiticsPrefabs)
        {
            item.show = false;
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("@coins", _score);
        PlayerPrefs.SetInt("@tutor", tutorComplete);
        PlayerPrefs.SetInt("@music", music);
        PlayerPrefs.SetInt("@sounds", sound);
        PlayerPrefs.SetString("@mode", mode);
        LoadDeadEnemies();
    }

    private void SetDeadCountToPrefabs()
    {
        foreach (var item in enemiesDeadList)
        {
            if (item.Value == 0)
                continue;

            EnemyAnalytics _tmp = analiticsPrefabs.Find(x => x.name.ToString() == item.Key);
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
        return analiticsPrefabs.FindAll(x => x.show && x.prefab != null).ConvertAll(x => x.prefab);
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
}
