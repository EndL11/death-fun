using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameProcess : MonoBehaviour
{
    public GameObject pausePanel;        
    public GameObject gameOverPanel;

    private Text scoreText;
    private Text enemiesText;

    public GameObject enemiesStatsParent;

    public GameObject bossUI = null;

    private void Awake()
    {
        if(GameSaving.instance != null)
            GameSaving.instance.deadEnemies = 0;

        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        enemiesText = GameObject.FindGameObjectWithTag("Enemies").GetComponent<Text>();
    }

    private void Start()
    {
        //  set timeScale to 1
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (GameSaving.instance != null)
        {
            GameSaving.instance.score = PlayerPrefs.GetInt("@coins", GameSaving.instance.score);

            GameSaving.instance.OnScoreChanged += UpdateScore;
            GameSaving.instance.OnEnemyDead += UpdateDeadCounter;
            GameSaving.instance.OnGameOver += GameOverHandler;
            GameSaving.instance.OnBossStart += OnBossStartHandler;


            scoreText.text = GameSaving.instance.score.ToString();
            enemiesText.text = GameSaving.instance.deadEnemies.ToString();
        }
        //testing score 
        GameSaving.instance.score = 999;
        scoreText.text = GameSaving.instance.score.ToString();


        //scoreText.text = GameSaving.instance.score.ToString();

        enemiesText.text = "0";
    }

    private void Update()
    {
        //  if pressed Escape and time not stopped
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1)
        {
            Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        pausePanel.SetActive(Time.timeScale == 0);
    }

    public void Restart()
    {
        GameSaving.instance.ClearPlayerPrefs();
  
        PlayerPrefs.SetInt("@saved", 0);
        pausePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        pausePanel.SetActive(false);
        Application.Quit();
    }

    private void UpdateDeadCounter()
    {
        enemiesText.text = GameSaving.instance.deadEnemies.ToString();
    }

    private void UpdateScore()
    {
        scoreText.text = GameSaving.instance.score.ToString();
    }

    private void GameOverHandler()
    {
        if(bossUI != null)
            bossUI.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;

        List<GameObject> prefabs = GameSaving.instance.GetAnalyticsObjects();

        foreach (var item in prefabs)
        {
            GameObject _instance = Instantiate(item, enemiesStatsParent.transform.position, Quaternion.identity, enemiesStatsParent.transform);
        }

        //  above add dead enemies to game over panel
        GameSaving.instance.ClearPlayerPrefs();
    }

    private void OnDestroy()
    {
        if (GameSaving.instance == null)
            return;

        GameSaving.instance.OnScoreChanged -= UpdateScore;
        GameSaving.instance.OnEnemyDead -=  UpdateDeadCounter;
        GameSaving.instance.OnGameOver -= GameOverHandler;
        GameSaving.instance.OnBossStart -= OnBossStartHandler;
    }

    public void GameOverRestart()
    {
        PlayerPrefs.SetInt("@saved", 0);

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (PlayerPrefs.GetString("@mode") == "Hard Mode")
        {
            SceneManager.LoadScene(2);
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        GameSaving.instance.ClearPlayerPrefs();
        SceneManager.LoadScene(0);
    }

    private void OnBossStartHandler()
    {
        bossUI.SetActive(true);
    }
}
