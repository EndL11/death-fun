using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succubus : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            if(PlayerPrefs.GetString("@mode") == "Normal Mode")
            {
                if (PlayerPrefs.GetFloat("@awardNormal", 0f) > GameSaving.instance.gameTime)
                    PlayerPrefs.SetFloat("@awardNormal", GameSaving.instance.gameTime);
            }
            else
            {
                if (PlayerPrefs.GetFloat("@awardHard", 0f) > GameSaving.instance.gameTime)
                    PlayerPrefs.SetFloat("@awardHard", GameSaving.instance.gameTime);
            }
                
            PlayerPrefs.DeleteKey("@level");
            Camera.main.GetComponent<Animator>().SetTrigger("Finish");
        }
    }
}
