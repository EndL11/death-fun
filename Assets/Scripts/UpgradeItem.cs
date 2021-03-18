using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public enum STATS {HP, HP_HALF, HP_FULL, DAMAGE, MAXHP};
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
}
