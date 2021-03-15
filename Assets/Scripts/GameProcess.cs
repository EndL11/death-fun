using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameProcess : MonoBehaviour
{
    public GameObject pausePanel;        

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

        GameSaving.instance.OnScoreChanged += UpdateScore;
        GameSaving.instance.OnEnemyDead += UpdateDeadCounter;

        scoreText.text = GameSaving.instance.score.ToString();
        enemiesText.text = GameSaving.instance.deadEnemies.ToString();
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

    void OnDestroy()
    {
        GameSaving.instance.OnScoreChanged -= UpdateScore;
        GameSaving.instance.OnEnemyDead -= this.UpdateDeadCounter;
    }
}
