using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SoundMenuItem{
    public Slider slider;
    public Toggle toggle;

}

public class Menu : MonoBehaviour
{
    public Toggle toggleMode;
    
#region SoundSettingItems
    public SoundMenuItem sound;
    public SoundMenuItem music;
#endregion

    public GameObject mainPanel;
    public GameObject optionsPanel;

    public GameObject loadTutorialButton;
    public GameObject continueButton;

    public GameObject playerMainMenu;

    public GameObject normalModeAward;
    public GameObject hardModeAward;

    public Sprite normalModeSprite;
    public Sprite hardModeSprite;

    private float _playerAttackLength = 0.6f;

    private const float miniVolumeLevel = -60f;

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

        float savedMusicVolume = PlayerPrefs.GetFloat("@music", 0f);
        float savedSoundVolume = PlayerPrefs.GetFloat("@sound", 0f);

        music.toggle.isOn = savedMusicVolume != miniVolumeLevel;
        music.slider.minValue = miniVolumeLevel;
        music.slider.value = savedMusicVolume;

        sound.toggle.isOn = savedSoundVolume != miniVolumeLevel;
        sound.slider.minValue = miniVolumeLevel;
        sound.slider.value = savedSoundVolume;

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
        yield return new WaitForSeconds(_playerAttackLength);
        if (PlayerPrefs.GetInt("@history", 0) == 0)
            SceneManager.LoadScene("StartHistory");
        else if (PlayerPrefs.GetInt("@tutor", 0) == 0)
            SceneManager.LoadScene("Tutorial");
        else
            SceneManager.LoadScene("level 1");
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
        yield return new WaitForSeconds(_playerAttackLength);
        Application.Quit();

    }
    public void Exit()
    {
        StartCoroutine(ExitWithAnimnation());
    }

    private IEnumerator OptionsWithAnimation()
    {
        yield return new WaitForSeconds(_playerAttackLength);
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
        SceneManager.LoadScene("Tutorial");
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

    public void MusicVolumeChange(Slider slider)
    {
        SoundMusicManager.instance.mixer.SetFloat("MusicVolume", slider.value);
        PlayerPrefs.SetFloat("@music", slider.value);
        CheckToToggleSoundSettings(music.toggle, slider.value);
    }

    public void SoundVolumeChange(Slider slider)
    {
        SoundMusicManager.instance.mixer.SetFloat("SFXVolume", slider.value);
        PlayerPrefs.SetFloat("@sound", slider.value);
        CheckToToggleSoundSettings(sound.toggle, slider.value);
    }

    private void CheckToToggleSoundSettings(Toggle toggle, float value){
        toggle.isOn = value != miniVolumeLevel;
    }
}
