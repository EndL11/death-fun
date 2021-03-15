using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Toggle toggleMode;
    public GameObject mainPanel;
    public GameObject optionsPanel;

    public GameObject playerMainMenu;

    private float playerAttackLength = 0.6f;

    private void Start()
    {
        Time.timeScale = 1;
        string mode = PlayerPrefs.GetString("@mode", "");
        if (mode == "")
        {
            mode = "Normal Mode";
            PlayerPrefs.SetString("@mode", mode);
        }
        Text modeName = toggleMode.GetComponentInChildren<Text>();
        modeName.text = mode;
        toggleMode.isOn = mode == "Hard Mode";
    }
    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(playerAttackLength);
        if (PlayerPrefs.GetInt("@tutor", 0) == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void Play()
    {
        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator ExitWithAnimnation()
    {
        yield return new WaitForSeconds(playerAttackLength);
        Application.Quit();

    }
    public void Exit()
    {
        StartCoroutine(ExitWithAnimnation());
    }

    private IEnumerator OptionsWithAnimation()
    {
        yield return new WaitForSeconds(playerAttackLength);
        optionsPanel.SetActive(true);
        mainPanel.SetActive(false);
        playerMainMenu.SetActive(false);
    }

    public void Options()
    {
        StartCoroutine(OptionsWithAnimation());
    }

    public void ChangeMode()
    {
        bool hardMode = toggleMode.isOn;
        Text modeName = toggleMode.GetComponentInChildren<Text>();
        modeName.text = hardMode ? "Hard Mode" : "Normal Mode";
        PlayerPrefs.SetString("@mode", modeName.text);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
