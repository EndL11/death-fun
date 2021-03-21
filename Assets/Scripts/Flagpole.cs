using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flagpole : Shaman
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        int diff = GameSaving.instance.enemiesCount - GameSaving.instance.deadEnemies;
        Text hint = hintText.GetComponent<Text>();
        if (diff > 0)
            hint.text = "You must kill all enemies!";
        else
            hint.text = "Good luck in the next level!";
        base.OnTriggerEnter2D(collision);
    }
}
