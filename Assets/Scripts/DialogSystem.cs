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

    private void Start()
    {
        anim = GetComponent<Animator>();
        Time.timeScale = 1;
        if (titles != null)
            titles.SetActive(false);

        dialogs[current].panel.SetActive(true);
        StartCoroutine(TypeSentence(sentences[current]));
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (var letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return null;
        }
    }

    private IEnumerator DisplayNextSentenceIEnum()
    {
        if (current == dialogs.Length - 1)
        {
            EndDialog();
            yield return null;
        }
        else
        {
            Dialog currentDialog = dialogs[current];
            current += 1;

            Dialog dialog = dialogs[current];
            if (dialog.needTransition && anim != null)
            {
                anim.SetTrigger("Transition");
                yield return new WaitForSeconds(.9f);
            }
            currentDialog.panel.SetActive(false);
            dialog.panel.SetActive(true);
            StopCoroutine(TypeSentence(sentences[current - 1]));
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
