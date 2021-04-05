using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GameSound{
    
    public enum SoundType {coin, triggerBoss, damagePlayer, spawnSphere, destroySphere, 
    playerAttack, damageEnemy, portal, bomberManExplosion, fireball, enemyDeath};
    public GameObject soundPrefab;  //  prefab with audioSource
}

[System.Serializable]
public struct GameMusic{
    
    public enum MusicType {gameMusic, menuMusic };
    public GameObject soundPrefab;  //  prefab with audioSource
}

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

    public void TriggerBossSoundPlay()
    {
        if(enableSounds)
            triggerBoss.Play();
    }

    public void TakeCoinSoundPlay()
    {
        if (enableSounds) coin.Play();
    }
	
	public void SpawnBlackHolePlay()
    {
        if (enableSounds) sphere.Play();
    }
	
	public void WooahPlay()
	{
        if (enableSounds) wooah.Play();
	}
	
	public void FlameBossPlay()
	{
        if (enableSounds) flameBoss.Play();
	}
	
	public void PunchPlay()
	{
        if (enableSounds) punch.Play();
	}
	
	public void ApplyDamagePlayerPlay()
	{
        if (enableSounds) damagePlayer.Play();
	}
		
	public void PortalPlay()
	{
        if (enableSounds) portal.Play();
	}
	
	public void ExplosionPlay()
	{
        if (enableSounds) explosion.Play();
	}
	
	public void DeathPlay()
	{
        if (enableSounds) death.Play();
	}
	
	public void SquahPlay()
	{
        if (enableSounds) squash.Play();
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
