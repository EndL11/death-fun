using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class GameProcess : MonoBehaviour
{
    public GameObject pausePanel;        
    public GameObject gameOverPanel;

    private Text scoreText;
    private Text enemiesText;

    public Transform enemiesStatsParent;
    public Transform enemiesStatsParent2;

    public GameObject bossUI = null;

    public GameObject finishPortal;
    public GameObject stairs;

    public GameObject flagpole;

    private void Awake()
    {
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        enemiesText = GameObject.FindGameObjectWithTag("EnemiesText").GetComponent<Text>();
        if (finishPortal != null)
            finishPortal.SetActive(false);
        if (flagpole != null)
            flagpole.SetActive(true);
        if (bossUI != null)
            bossUI.SetActive(false);
        if (stairs != null)
            stairs.SetActive(false);
    }

    private void Start()
    {
		SoundMusicManager.instance.PortalPlay();
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
            GameSaving.instance.OnBossDie += OnBossEndHandler;
            GameSaving.instance.OnEndLevel += OnEndLevelHandler;

            GameSaving.instance.enemies = GameObject.FindGameObjectsWithTag("Enemies").ToList();
            GameSaving.instance.enemiesCount = GameSaving.instance.enemies.Count;
            GameSaving.instance.deadEnemies = 0;
        }
        enemiesText.text = $"{GameSaving.instance.deadEnemies} / {GameSaving.instance.enemiesCount}";
        scoreText.text = GameSaving.instance.score.ToString();
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
        enemiesText.text = $"{GameSaving.instance.deadEnemies} / {GameSaving.instance.enemiesCount}";
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
        Transform parent = enemiesStatsParent;
        int counter = 0;
        foreach (var item in prefabs)
        {
            if(counter == 12)
            {
                parent = enemiesStatsParent2;
            }
            GameObject _instance = Instantiate(item, parent.position, Quaternion.identity, parent);
            counter += 1;
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
        GameSaving.instance.OnBossDie -= OnBossEndHandler;
        GameSaving.instance.OnEndLevel -= OnEndLevelHandler;
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

    private void OnEndLevelHandler()
    {
        if(finishPortal != null)
            finishPortal.SetActive(true);

        if(flagpole != null)
            flagpole.SetActive(false);
    }

    private void OnBossEndHandler()
    {
        if (stairs != null)
            stairs.SetActive(true);
    }
}
