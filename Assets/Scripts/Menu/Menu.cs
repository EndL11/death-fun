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

    public GameObject loadTutorialButton;
    public GameObject continueButton;

    public GameObject playerMainMenu;

    public GameObject normalModeAward;
    public GameObject hardModeAward;

    public Sprite normalModeSprite;
    public Sprite hardModeSprite;

    private float playerAttackLength = 0.6f;

    private IEnumerator Start()
    {
        if(GameSaving.instance == null && SoundMusicManager.instance.showPreloader)
            yield return new WaitForSeconds(3.8f);

        yield return null;

        SoundMusicManager.instance.showPreloader = false;
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
        loadTutorialButton.SetActive(false);
        continueButton.SetActive(false);

        if (PlayerPrefs.HasKey("@level"))
            continueButton.SetActive(true);

        if (PlayerPrefs.GetInt("@history", 0) == 1)
            loadTutorialButton.SetActive(true);

        //  setting awards
        float hardModeTime = PlayerPrefs.GetFloat("@awardHard", 0f);
        float normalModeTime = PlayerPrefs.GetFloat("@awardNormal", 0f);
        if (hardModeTime != 0f)
        {
            hardModeAward.GetComponentInChildren<Image>().sprite = hardModeSprite;
            hardModeAward.GetComponentInChildren<Text>().text = ConvertGameTimeToString(hardModeTime);
        }

        if (normalModeTime != 0f)
        {
            normalModeAward.GetComponentInChildren<Image>().sprite = normalModeSprite;
            normalModeAward.GetComponentInChildren<Text>().text = ConvertGameTimeToString(normalModeTime);
        }
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
        PlayerPrefs.DeleteKey("@level");
        PlayerPrefs.SetInt("@saved", 0);
        PlayerPrefs.DeleteKey("@coins");
        PlayerPrefs.DeleteKey("@currentGameTime");
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
        continueButton.SetActive(false);
        PlayerPrefs.DeleteKey("@level");
        PlayerPrefs.DeleteKey("@complete");
        PlayerPrefs.DeleteKey("@saved");
        PlayerPrefs.DeleteKey("@currentGameTime");
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

    public void ContinueClick()
    {
        SoundMusicManager.instance.backgroundMenuMusicStop();
        PlayerPrefs.SetInt("@complete", 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("@level"));
    }

    private string ConvertGameTimeToString(float gameTime)
    {
        if (gameTime <= 0f)
            return "00:00:00";

        int timeValue = (int)gameTime;
        string time = "";
        int hours = timeValue / 3600;
        time += hours >= 10 ? $"{hours}:" : $"0{hours}:";
        timeValue -= hours * 3600;
        int minutes = timeValue / 60;
        time += minutes >= 10 ? $"{minutes}:" : $"0{minutes}:";
        timeValue -= minutes * 60;
        int seconds = timeValue;
        time += seconds > 0 ? seconds >= 10 ? $"{seconds}" : $"0{seconds}" : "00";
        return time;
    }
}
