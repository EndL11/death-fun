using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcess : MonoBehaviour
{
    public GameObject pausePanel;
    public Transform startPortal;
    public GameObject player;

    private void Awake()
    {
        Instantiate(player, startPortal.position, Quaternion.identity);
    }

    void Start()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }


    void Update()
    {
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
}
