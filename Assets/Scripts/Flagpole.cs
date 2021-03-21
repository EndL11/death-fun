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
        hint.text = "You have to kill ";
        if (diff > 1)
            hint.text += $"{diff} enemies";
        else if (diff == 1)
            hint.text += $"{diff} enemy";
        else
            hint.text = "Requirements complete!";
        base.OnTriggerEnter2D(collision);
    }
}
