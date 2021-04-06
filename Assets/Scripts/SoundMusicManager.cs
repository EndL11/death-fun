using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager instance;
	public AudioSource backgroundMusic;
	public AudioSource backgroundMenuMusic;

    public AudioMixer mixer;
	
    private bool enableMusic = true;
    private bool enableSounds = true;

    public bool Music
    {
        get { return enableMusic; }
        set {
            enableMusic = value;
            if (enableMusic)
            {
                backgroundMenuMusic.Play();
            }
            else
            {
                backgroundMenuMusic.Stop();
            }
        }
    }

    public bool Sound
    {
        get { return enableSounds; }
        set { enableSounds = value;  }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        enableMusic = PlayerPrefs.GetInt("@music", 1) == 1 ? true : false;
        enableSounds = PlayerPrefs.GetInt("@sounds", 1) == 1 ? true : false;
    }


    //  test
    private void Start() {
        //mixer.SetFloat("MusicVolume", -80f);
        //mixer.SetFloat("SFXVolume", -80f);
    }
	
	public void backgroundMenuMusicPlay()
	{
        if (enableMusic) backgroundMenuMusic.Play();
	}
	
	public void backgroundMusicPlay()
	{
        if (enableMusic) backgroundMusic.Play();
	}
	
	public void backgroundMenuMusicStop()
	{
        if (enableMusic) backgroundMenuMusic.Pause();
	}
	
	public void backgroundMusicStop()
	{
        if (enableMusic) backgroundMusic.Pause();
	}
	
	
    private void OnDestroy()
    {
        PlayerPrefs.SetInt("@music", enableMusic ? 1 : 0);
        PlayerPrefs.SetInt("@sounds", enableSounds ? 1 : 0);
    }
}
