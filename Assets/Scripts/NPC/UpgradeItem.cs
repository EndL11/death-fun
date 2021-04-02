using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public enum STATS {HP, HP_HALF, HP_FULL, DAMAGE, MAXHP, SPHERE_DELAY, SPHERE_RADIUS, SPHERE_DAMAGE};
    [SerializeField] private STATS _statsIdentificator;
    [SerializeField] private float _value;
    [SerializeField] private string _description = "Get HP $$%";
    [SerializeField] private int _cost = 50;

    public STATS Identificator
    {
        get { return _statsIdentificator; }
    }

    public float Value
    {
        get { return _value; }
    }

    public int Cost
    {
        get { return _cost; }
    }

    public string Description
    {
        get { return _description; }
    }
}
