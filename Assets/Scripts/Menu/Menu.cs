﻿using System.Collections;
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

    public Text difficultyButtonText;

    private float _playerAttackLength = 0.6f;

    private const float minVolumeLevel = 0f;
    private const float maxVolumeLevel = 1f;

    [Header("Coefficient")]
    public float easy = 1f;
    public float medium = 2f;
    public float hard = 3f;

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

        string difficultyMode = PlayerPrefs.GetString("@difficulty", "Easy");

        float savedMusicVolume = PlayerPrefs.GetFloat("@music", maxVolumeLevel);
        float savedSoundVolume = PlayerPrefs.GetFloat("@sound", maxVolumeLevel);

        music.toggle.isOn = savedMusicVolume != minVolumeLevel;
        music.slider.minValue = minVolumeLevel;
        music.slider.value = savedMusicVolume;
        music.slider.maxValue = maxVolumeLevel;

        sound.toggle.isOn = savedSoundVolume != minVolumeLevel;
        sound.slider.minValue = minVolumeLevel;
        sound.slider.value = savedSoundVolume;
        sound.slider.maxValue = maxVolumeLevel;

        difficultyButtonText.text = difficultyMode;

        loadTutorialButton.SetActive(false);
        continueButton.SetActive(false);

        if (PlayerPrefs.HasKey("@level") && PlayerPrefs.GetInt("@level", 0) >= 4)
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
        SoundMusicManager.instance.ChangeMusicVolume(slider.value);
        PlayerPrefs.SetFloat("@music", slider.value);
        CheckToToggleSoundSettings(music.toggle, slider.value);
    }

    public void SoundVolumeChange(Slider slider)
    {
        SoundMusicManager.instance.ChangeSFXVolume(slider.value);
        PlayerPrefs.SetFloat("@sound", slider.value);
        CheckToToggleSoundSettings(sound.toggle, slider.value);
    }

    private void CheckToToggleSoundSettings(Toggle toggle, float value){
        toggle.isOn = value != minVolumeLevel;
    }

    public void ChangeDifficulty(){
        string current = difficultyButtonText.text;
        if(current == "Easy"){
            current = "Medium";
            PlayerPrefs.SetFloat("@koef", medium);
        }
        else if(current == "Medium"){
            current = "Hard";
            PlayerPrefs.SetFloat("@koef", hard);
        }
        else{
            current = "Easy";
            PlayerPrefs.SetFloat("@koef", easy);
        }
        difficultyButtonText.text = current;
        PlayerPrefs.SetString("@difficulty", current);
    }
}
