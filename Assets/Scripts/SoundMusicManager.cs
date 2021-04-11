using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager instance;

    private List<AudioSource> _sfx = new List<AudioSource>();
    private List<AudioSource> _musics = new List<AudioSource>();
#region SFX
    public AudioSource triggerBoss;
    public AudioSource coin;
    public AudioSource sphere;
    public AudioSource wooah;
    public AudioSource punch;
    public AudioSource flameBoss;
    public AudioSource damagePlayer;
    public AudioSource portal;
    public AudioSource explosion;
    public AudioSource death;
    public AudioSource squash;
#endregion
#region Music
    public AudioSource backgroundMusic;
    public AudioSource backgroundMenuMusic;
#endregion
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        _musicVolume = PlayerPrefs.GetFloat("@music", 1f);
        _sfxVolume = PlayerPrefs.GetFloat("@sounds", 1f);

        SetUpMusicsList();
        SetUpSFXList();

        ChangeMusicVolume(_musicVolume);
        ChangeSFXVolume(_sfxVolume);
    }

    private void SetUpMusicsList(){
        _musics.Add(backgroundMenuMusic);
        _musics.Add(backgroundMusic);
    }

    private void SetUpSFXList(){
        IEnumerable<AudioSource> list = new List<AudioSource>(){triggerBoss, coin, sphere, wooah, punch, 
        flameBoss, damagePlayer, portal, explosion, death, squash};
        _sfx.AddRange(list);
    }

    public void TriggerBossSoundPlay()
    {
        triggerBoss.Play();
    }

    public void TakeCoinSoundPlay()
    {
        coin.Play();
    }

    public void SpawnBlackHolePlay()
    {
        sphere.Play();
    }

    public void WooahPlay()
    {
        wooah.Play();
    }

    public void FlameBossPlay()
    {
        flameBoss.Play();
    }

    public void PunchPlay()
    {
        punch.Play();
    }

    public void ApplyDamagePlayerPlay()
    {
        damagePlayer.Play();
    }

    public void PortalPlay()
    {
        portal.Play();
    }

    public void ExplosionPlay()
    {
        explosion.Play();
    }

    public void DeathPlay()
    {
        death.Play();
    }

    public void SquahPlay()
    {
        squash.Play();
    }

    public void backgroundMenuMusicPlay()
    {
        backgroundMenuMusic.Play();
    }

    public void backgroundMusicPlay()
    {
        backgroundMusic.Play();
    }

    public void backgroundMenuMusicStop()
    {
        backgroundMenuMusic.Pause();
    }

    public void backgroundMusicStop()
    {
        backgroundMusic.Pause();
    }

    public void ChangeMusicVolume(float volume)
    {
        _musicVolume = volume;
        foreach (var item in _musics)
        {
            item.volume = volume;
        }
    }

    public void ChangeSFXVolume(float volume)
    {
        _sfxVolume = volume;
        foreach (var item in _sfx)
        {
            item.volume = volume;
        }
    }


    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("@music", _musicVolume);
        PlayerPrefs.SetFloat("@sounds", _sfxVolume );
    }
}
