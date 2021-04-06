using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class GameProcess : MonoBehaviour
{
#region Panels
    public GameObject pausePanel;        
    public GameObject gameOverPanel;
#endregion
#region Texts
    private Text _scoreText;
    private Text _enemiesText;
    public Text timerText;
#endregion
#region Analytics wrappers
    public Transform enemiesStatsParent;
    public Transform enemiesStatsParent2;
#endregion
    public GameObject bossUI = null;

#region Level objects
    public GameObject finishPortal;
    public GameObject stairs;
    public GameObject flagpole;
#endregion
    public float currectGameTime;

    public AudioSource portalStartSFX;

    private void Awake()
    {
        _scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        _enemiesText = GameObject.FindGameObjectWithTag("EnemiesText").GetComponent<Text>();
        if (finishPortal != null)
            finishPortal.SetActive(false);
        if (flagpole != null)
            flagpole.SetActive(true);
        if (bossUI != null)
            bossUI.SetActive(false);
        if (stairs != null)
            stairs.SetActive(false);

        currectGameTime = PlayerPrefs.GetFloat("@currentGameTime", 0f);
        if (SceneManager.GetActiveScene().name == "Tutorial")
            timerText.gameObject.SetActive(false);
        else
            timerText.gameObject.SetActive(true);

    }

    private void Start()
    {
        if(!SoundMusicManager.instance.backgroundMusic.isPlaying)
		    SoundMusicManager.instance.backgroundMusicPlay();
		portalStartSFX.Play();
        //  set timeScale to 1
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (GameSaving.instance != null)
        {
            GameSaving.instance.score = PlayerPrefs.GetInt("@coins", 0);

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
        _enemiesText.text = $"{GameSaving.instance.deadEnemies} / {GameSaving.instance.enemiesCount}";
        _scoreText.text = GameSaving.instance.score.ToString();
        if(PlayerPrefs.GetString("@mode") == "Normal Mode" && SceneManager.GetActiveScene().name != "Tutorial")
            PlayerPrefs.SetInt("@level", SceneManager.GetActiveScene().buildIndex);

        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            InvokeRepeating("UpdateTimerText", 0f, .5f);
        }
    }

    private void Update()
    {
        //  make pause or unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
        currectGameTime += Time.deltaTime;
    }

    private void UpdateTimerText()
    {
        timerText.text = GameSaving.instance.ConvertGameTimeToString(currectGameTime);
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        pausePanel.SetActive(Time.timeScale == 0);
    }

    public void Restart()
    {
        ClearStatsByMode();
        pausePanel.SetActive(false);
        if(PlayerPrefs.GetString("@mode") == "Normal Mode")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        currectGameTime = 0f;
        PlayerPrefs.DeleteKey("@currentGameTime");
        SceneManager.LoadScene("level 1");
    }

    public void Exit()
    {
        if(PlayerPrefs.GetString("@mode") == "Hard Mode")
            GameSaving.instance.ClearPlayerPrefs();

        pausePanel.SetActive(false);
        Application.Quit();
    }

    private void UpdateDeadCounter()
    {
        _enemiesText.text = $"{GameSaving.instance.deadEnemies} / {GameSaving.instance.enemiesCount}";
    }

    private void UpdateScore()
    {
        _scoreText.text = GameSaving.instance.score.ToString();
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
        ClearStatsByMode();
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

        if(SceneManager.GetActiveScene().name != "Tutorial")
            PlayerPrefs.SetFloat("@currentGameTime", currectGameTime);
    }

    public void GameOverRestart()
    {
        if (PlayerPrefs.GetString("@mode") == "Hard Mode")
        {
            SceneManager.LoadScene("level 1");
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ClearStatsByMode()
    {
        if (PlayerPrefs.GetString("@mode") == "Hard Mode")
        {
            GameSaving.instance.ClearPlayerPrefs();
            PlayerPrefs.SetInt("@saved", 0);
        }
    }

    public void Menu()
    {
        if(gameOverPanel.activeSelf)
            PlayerPrefs.DeleteKey("@level");

        if(PlayerPrefs.GetString("@mode") == "Hard Mode")
            ClearStatsByMode();

        SoundMusicManager.instance.backgroundMusicStop();
        SceneManager.LoadScene("Menu");
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
