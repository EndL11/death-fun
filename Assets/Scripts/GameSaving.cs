using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public struct PlayerStats
{
    public float damage;
    public float hp;
    public float maxHp;
}

public class GameSaving : MonoBehaviour
{
    public event Action OnScoreChanged = () => { };
    public event Action OnEnemyDead = () => { };
    public event Action OnGameOver = () => { };
    public static GameSaving instance;
    public PlayerStats playerStats;
    public int score = 0;
    public int deadEnemies = 0;
    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex != 1)
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

    public void AddScore(int value)
    {
        score += value;
        OnScoreChanged();
    }
	

    public void EnemyDead()
    {
        deadEnemies += 1;
        OnEnemyDead();
    }

    public void GameOver()
    {
        OnGameOver();
    }

    public void SaveCompleteTutorial()
    {
        PlayerPrefs.SetInt("@tutor", 1);
    }

}
