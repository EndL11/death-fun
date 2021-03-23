using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Toggle toggleMode;
    public Toggle toggleMusic;
    public Toggle toggleSound;
    public GameObject mainPanel;
    public GameObject optionsPanel;

    public GameObject playerMainMenu;

    private float playerAttackLength = 0.6f;

    private void Start()
    {
		SoundMusicManager.instance.backgroundMenuMusicPlay();
        Time.timeScale = 1;
        string mode = PlayerPrefs.GetString("@mode", "");
        if (mode == "")
        {
            mode = "Normal Mode";
            PlayerPrefs.SetString("@mode", mode);
        }
        toggleMode.isOn = mode == "Hard Mode";

        toggleMusic.isOn = SoundMusicManager.instance.Music;
        toggleSound.isOn = SoundMusicManager.instance.Sound;
    }

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(playerAttackLength);
        if (PlayerPrefs.GetInt("@history", 0) == 0)
            SceneManager.LoadScene(14);
        else if (PlayerPrefs.GetInt("@tutor", 0) == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void Play()
    {
		SoundMusicManager.instance.backgroundMenuMusicStop();
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
        bool isOn = toggleMode.isOn;
        string modeName = !isOn ? "Hard Mode" : "Normal Mode";
        PlayerPrefs.SetString("@mode", modeName);
        toggleMode.isOn = !isOn;
    }

    public void LoadTutorial()
    {
        SoundMusicManager.instance.backgroundMenuMusicStop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToggleMusic()
    {
        toggleMusic.isOn = !toggleMusic.isOn;
        SoundMusicManager.instance.Music = toggleMusic.isOn;
    }

    public void ToggleSound()
    {
        toggleSound.isOn = !toggleSound.isOn;
        SoundMusicManager.instance.Sound = toggleSound.isOn;
    }
}
