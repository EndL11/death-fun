using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager instance;
    public AudioSource triggerBoss;
    public AudioSource damageBoss;

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
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void TriggerBossSoundPlay()
    {
        triggerBoss.Play();
    }

    public void DamageBossSoundPlay()
    {
        damageBoss.Play();
    }
}
