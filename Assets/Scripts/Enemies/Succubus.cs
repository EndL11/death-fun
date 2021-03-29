using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succubus : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))
        {
            float gameTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameProcess>().currectGameTime;
            if (PlayerPrefs.GetString("@mode") == "Normal Mode")
            {
                if (PlayerPrefs.GetFloat("@awardNormal", 0f) == 0f)
                    PlayerPrefs.SetFloat("@awardNormal", gameTime);
                else if (PlayerPrefs.GetFloat("@awardNormal") > gameTime)
                    PlayerPrefs.SetFloat("@awardNormal", gameTime);
            }
            else
            {
                if(PlayerPrefs.GetFloat("@awardHard", 0f) == 0f)
                    PlayerPrefs.SetFloat("@awardHard", gameTime);
                else if (PlayerPrefs.GetFloat("@awardHard") > gameTime)
                    PlayerPrefs.SetFloat("@awardHard", gameTime);
            }
            PlayerPrefs.DeleteKey("@currentGameTime");
            PlayerPrefs.DeleteKey("@level");
            Camera.main.GetComponent<Animator>().SetTrigger("Finish");
        }
    }
}
