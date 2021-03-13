using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public enum STATS {HP_SMALL, HP_MEDIUM, HP_FULL};
    [SerializeField] private STATS statsIdentificator;
    [SerializeField] private float value;

    public STATS Identificator
    {
        get { return statsIdentificator; }
    }

    public float Value
    {
        get { return value; }
    }

    private void Start()
    {
        //  recover 25% of max hp
        if(statsIdentificator == STATS.HP_SMALL)
        {
            value = .25f;
        }
        //  recover 50% of max hp
        else if (statsIdentificator == STATS.HP_MEDIUM)
        {
            value = .5f;
        }
        //  recover 100% of max hp
        else if (statsIdentificator == STATS.HP_FULL)
        {
            value = 1f;
        }
    }
}
