using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager instance;
    public AudioSource triggerBoss;
    public AudioSource damageBoss;
    public AudioSource coin;

    private bool enableMusic = true;
    private bool enableSounds = true;

    public bool Music
    {
        get { return enableMusic; }
        set { enableMusic = value; }
    }

    public bool Sound
    {
        get { return enableSounds; }
        set { enableSounds = value; }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        enableMusic = PlayerPrefs.GetInt("@music", 1) == 1 ? true : false;
        enableSounds = PlayerPrefs.GetInt("@sounds", 1) == 1 ? true : false;
    }

    public void TriggerBossSoundPlay()
    {
        triggerBoss.Play();
    }

    public void DamageBossSoundPlay()
    {
        damageBoss.Play();
    }

    public void TakeCoinSoundPlay()
    {
        coin.Play();
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("@music", enableMusic ? 1 : 0);
        PlayerPrefs.SetInt("@sounds", enableSounds ? 1 : 0);
    }
}
