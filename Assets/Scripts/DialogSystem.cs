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
    public GameObject continueButton;

    public string[] sentences;
    private int _current = 0;
    public Dialog[] dialogs;

    #region EndHistorySettings
    public bool isEnd = true;
    public GameObject titlesBackground;
    public GameObject titles;
    #endregion

    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        Time.timeScale = 1;

        if (titles != null)
            titles.SetActive(false);

        dialogs[_current].panel.SetActive(true);
        continueButton.SetActive(false);
        StartCoroutine(TypeSentence(sentences[_current]));
    }

    public void LoadMenu()
    {
        SoundMusicManager.instance.backgroundMusicStop();
        SceneManager.LoadScene("Menu");
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
        if (_current == dialogs.Length - 1)
        {
            EndDialog();
        }
        else
        {
            if(_current != dialogs.Length - 2)
                continueButton.SetActive(false);

            Dialog currentDialog = dialogs[_current];
            _current += 1;

            Dialog nextDialog = dialogs[_current];
            if (nextDialog.needTransition && _anim != null)
            {
                _anim.SetTrigger("Transition");
                yield return new WaitForSeconds(1.3f);
            }
            currentDialog.panel.SetActive(false);
            nextDialog.panel.SetActive(true);

            if (_current != sentences.Length)
                StartCoroutine(TypeSentence(sentences[_current]));
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
            SceneManager.LoadScene("Tutorial");
        }
    }
}
