﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
    public static GameSaving instance;
    public PlayerStats playerStats;
    public int score = 0;
    public int deadEnemies = 0;
    void Awake()
    {
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

}
