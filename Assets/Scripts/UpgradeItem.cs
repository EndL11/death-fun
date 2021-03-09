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
        if(statsIdentificator == STATS.HP_SMALL)
        {
            value = .25f;
        }
        else if (statsIdentificator == STATS.HP_MEDIUM)
        {
            value = .5f;
        }
        else if (statsIdentificator == STATS.HP_FULL)
        {
            value = 1f;
        }
    }
}
