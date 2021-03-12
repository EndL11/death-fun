using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    //  damage on static
    public float staticDamage = 10f;
    //  damage on action saw
    public float increasedDamage = 30f;
    //  up force on static
    public float staticUpForce = 2f;
    //  up force on action saw
    public float increasedUpForce = 3.5f;

    private float currentDamage;
    private float currentUpForce;

    private void Start()
    {
        currentDamage = staticDamage;
        currentUpForce = staticUpForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().ApplyDamage(currentDamage, transform.up * currentUpForce);
        }
    }

    public void SetStaticValues()
    {
        currentDamage = staticDamage;
        currentUpForce = staticUpForce;
    }

    public void SetIncreasedValues()
    {
        currentDamage = increasedDamage;
        currentUpForce = increasedUpForce;
    }
}
