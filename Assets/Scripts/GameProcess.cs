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

    private void Awake()
    {
        if(GameSaving.instance != null)
            GameSaving.instance.deadEnemies = 0;

        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        enemiesText = GameObject.FindGameObjectWithTag("Enemies").GetComponent<Text>();
    }

    void Start()
    {
        //  set timeScale to 1
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (GameSaving.instance != null)
        {
            GameSaving.instance.score = PlayerPrefs.GetInt("@coint", GameSaving.instance.score);

            GameSaving.instance.OnScoreChanged += UpdateScore;
            GameSaving.instance.OnEnemyDead += UpdateDeadCounter;
            GameSaving.instance.OnGameOver += GameOverHandler;

            scoreText.text = GameSaving.instance.score.ToString();
            enemiesText.text = GameSaving.instance.deadEnemies.ToString();
        }
        scoreText.text = GameSaving.instance.score.ToString();
        enemiesText.text = "0";
    }


    void Update()
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
        PlayerPrefs.GetInt("@saved", 0);
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
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    void OnDestroy()
    {
        if (GameSaving.instance == null)
            return;

        GameSaving.instance.OnScoreChanged -= UpdateScore;
        GameSaving.instance.OnEnemyDead -=  UpdateDeadCounter;
        GameSaving.instance.OnGameOver -= GameOverHandler;
    }

    public void GameOverRestart()
    {
        PlayerPrefs.GetInt("@saved", 0);
        if (PlayerPrefs.GetString("@mode") == "Hard Mode")
        {
            SceneManager.LoadScene(2);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
