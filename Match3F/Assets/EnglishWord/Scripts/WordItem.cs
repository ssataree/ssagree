using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class WordItem : MonoBehaviour, IPointerClickHandler
{
    public Text wordText;

    public Image backGroundImage;

    public UnityEvent onPointerClickEvent;

    public GameObject[] buttons;


    private Action<string, Text> onTranslate;
    

    public void Initialize(string word, Action<string, Text> onTranslate)
    {
        wordText.text = word;
        this.onTranslate = onTranslate;
    }

    

    private void SetActiveButtons(bool value)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(value);
        }
    }

    private void OnSelect()
    {
        IsSelect = true;
        Color color = backGroundImage.color;
        backGroundImage.color = color * 0.5f;
        SetActiveButtons(true);
    }

    private void OnDeSelect()
    {
        IsSelect = false;
        backGroundImage.color = Color.white;
        SetActiveButtons(false);
    }

    public bool IsSelect { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick : " + IsSelect);
        onPointerClickEvent?.Invoke();
        if (IsSelect)
            OnDeSelect();
        else
            OnSelect();
    }

    public string Word
    {
        get
        {
            if (string.IsNullOrEmpty(word))
                return wordText.text;
            else
                return word;
        }
    }

    private string word;

    public void OnPressTranslateButton()
    {
        if (string.IsNullOrEmpty(Word))
        {
            word = wordText.text;
            onTranslate(word, wordText);
        }
        else
        {
            wordText.text = Word;
            word = string.Empty;
        }
    }

    public void OnPressOpenUrlButton()
    {
        string word = wordText.text;

        string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + "auto" + "&tl=" + "ko" + "&dt=t&q=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(word);

        Application.OpenURL(url);
    }
}
