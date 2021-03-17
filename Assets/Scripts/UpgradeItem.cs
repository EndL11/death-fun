using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public enum STATS {HP_SMALL, HP_MEDIUM, HP_FULL};
    [SerializeField] private STATS statsIdentificator;
    [SerializeField] private float value;
    [SerializeField] private string description = "Get HP $$%";
    [SerializeField] private int cost = 50;

    public STATS Identificator
    {
        get { return statsIdentificator; }
    }

    public float Value
    {
        get { return value; }
    }

    public int Cost
    {
        get { return cost; }
    }

    public string Description
    {
        get { return description; }
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
