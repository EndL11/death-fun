using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Dialog
{
    public GameObject panel;
    public bool needTransition;
}

public class DialogSystem : MonoBehaviour
{
    public Text dialogText;
    public GameObject dialogPanel;

    public string[] sentences;
    private int current = 0;

    public Dialog[] dialogs;

    public bool isEnd = true;
    public GameObject titlesBackground;
    public GameObject titles;

    private Animator anim;

    public GameObject continueButton;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Time.timeScale = 1;
        if (titles != null)
            titles.SetActive(false);

        dialogs[current].panel.SetActive(true);
        continueButton.SetActive(false);
        StartCoroutine(TypeSentence(sentences[current]));
    }

    public void LoadMenu()
    {
        SoundMusicManager.instance.backgroundMusicStop();
        SceneManager.LoadScene(0);
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (var letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.015f);
        }
        continueButton.SetActive(true);
    }

    private IEnumerator DisplayNextSentenceIEnum()
    {
        if (current == dialogs.Length - 1)
        {
            EndDialog();
        }
        else
        {
            if(current != dialogs.Length - 2)
            continueButton.SetActive(false);

            Dialog currentDialog = dialogs[current];
            current += 1;

            Dialog nextDialog = dialogs[current];
            if (nextDialog.needTransition && anim != null)
            {
                anim.SetTrigger("Transition");
                yield return new WaitForSeconds(1.3f);
            }
            currentDialog.panel.SetActive(false);
            nextDialog.panel.SetActive(true);
            if (current != sentences.Length)
                StartCoroutine(TypeSentence(sentences[current]));
            else
                dialogText.text = "";
        }
    }

    public void DisplayNextSentense()
    {
        StartCoroutine(DisplayNextSentenceIEnum());
    }

    protected void EndDialog()
    {
        if (isEnd)
        {
            titlesBackground.SetActive(true);
            titles.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("@history", 1);
            SceneManager.LoadScene(1);
        }
    }
}
