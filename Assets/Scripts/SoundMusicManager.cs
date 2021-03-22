using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager instance;
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
	public AudioSource backgroundMusic;
	public AudioSource backgroundMenuMusic;
	
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
		backgroundMenuMusic.Stop();
	}
	
	public void backgroundMusicStop()
	{
		backgroundMusic.Stop();
	}
	
	
	//SoundMusicManager.instance.backgroundMenuMusicPlay());
    private void OnDestroy()
    {
        PlayerPrefs.SetInt("@music", enableMusic ? 1 : 0);
        PlayerPrefs.SetInt("@sounds", enableSounds ? 1 : 0);
    }
}
