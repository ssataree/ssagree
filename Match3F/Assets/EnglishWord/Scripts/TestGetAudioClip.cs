using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using SimpleJSON;


public class TestGetAudioClip : MonoBehaviour
{
    public Text logText;

    public AudioSource audioSource;

    public AudioType audioType;

    public InputField translateField;

    public Text translateText;

    public GameObject wordItemPrefab;

    public RectTransform wordsParent;

    public float wordItemHeight;



    private bool isProcess;

    public void OnClick(AudioType audioType)
    {
        if (isProcess)
            return;

        //StartCoroutine(DownLoadTheAudio(audioType));
    }


    public void TranslateWord(string sourceText, Text wordText)
    {
        if (isTranslate || isDownloadAudio)
            return;

        StartCoroutine(TranslateWord("ko", sourceText, wordText));
        StartCoroutine(DownLoadTheAudio(AudioType.MPEG, sourceText));
    }

    public void OnPressTranslateButton()
    {
        string sourceText = translateField.text;
        if (string.IsNullOrEmpty(sourceText))
            return;

        TranslateWord(sourceText, translateText);
    }


    private List<string> extractWordList = new List<string>();

    private List<WordItem> wordItems = new List<WordItem>();

    public void OnPressExtractButton()
    {
        string fieldText = translateField.text;
        string[] entities = fieldText.Split(new char[] { ',', ';', '.', ' ', ':'}, System.StringSplitOptions.RemoveEmptyEntries);
        if (null == entities)
            return;

        for (int i = 0; i < entities.Length; i++)
        {
            Debug.Log(entities[i]);
            if (WordManager.Instance.IsRegistedWord(entities[i]) || extractWordList.Contains(entities[i]))
                continue;

            var wordItem = Instantiate(wordItemPrefab, wordsParent).GetComponent<WordItem>();
            wordItem.Initialize(entities[i], TranslateWord);
            wordItems.Add(wordItem);
            extractWordList.Add(entities[i]);
        }

        wordsParent.sizeDelta = new Vector2(0f, entities.Length * wordItemHeight);
    }

    public void OnPressAddDeActiveButton()
    {
        List<WordItem> removeItems = new List<WordItem>();
        for (int i = 0; i < wordItems.Count; i++)
        {
            var iterWordItem = wordItems[i];
            if(iterWordItem.IsSelect)
            {
                WordManager.Instance.AddDeActiveWord(iterWordItem.wordText.text);
                removeItems.Add(iterWordItem);
            }
        }

        for (int i = 0; i < removeItems.Count; i++)
        {
            wordItems.Remove(removeItems[i]);
            Destroy(removeItems[i].gameObject);
        }
    }


    public void OnPressRegistButton()
    {
        if (wordItems.Count <= 0)
        {
            WordManager.Instance.AddDeActiveWord(translateField.text);
            ClearInputField();
            return;
        }

        for (int i = 0; i < wordItems.Count; i++)
        {
            var iterWordItem = wordItems[i];
            if (iterWordItem.IsSelect)
                WordManager.Instance.AddDeActiveWord(iterWordItem.wordText.text);
            else
                WordManager.Instance.AddActiveWord(iterWordItem.wordText.text);
        }
    }

    public void OnPressClearInputFieldButton()
    {
        ClearInputField();
    }

    public void OnPressExitButton()
    {
        WordManager.Instance.SaveData();
        Application.Quit();
    }



    public void ClearWords()
    {
        extractWordList.Clear();
        for (int i = 0; i < wordItems.Count; i++)
        {
            Destroy(wordItems[i].gameObject);
        }
        wordItems.Clear();
    }

    public void ClearLog()
    {
        logText.text = string.Empty;
        count = 0;
    }

    public void GetExcel()
    {
        if (isProcess)
            return;

        StartCoroutine(DownLoadTheExcel());
    }


    private void ClearInputField()
    {
        translateField.text = string.Empty;
    }

    private bool isTranslate;

    private IEnumerator TranslateWord(string targetLang, string sourceText, Text wordText)
    {
        isTranslate = true;

        string sourceLang = "auto";

        string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(sourceText);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                translateText.text = webRequest.error;
            }
            else
            {
                var N = JSONNode.Parse(webRequest.downloadHandler.text);
                //translateText.text = N[0][0][0];
                wordText.text = N[0][0][0];
            }

            isTranslate = false;
        }
    }


    private IEnumerator DownLoadTheExcel()
    {
        isProcess = true;
        string ulr1 = "https://docs.google.com/spreadsheets/d/1AuNassaY00_56XxSlBB70hndQ4ScULvJxptL-dMm638/edit#gid=0";
        string ulr2 = "https://docs.google.com/spreadsheets/d/1AuNassaY00_56XxSlBB70hndQ4ScULvJxptL-dMm638/edit?usp=sharing";
        using (UnityWebRequest www = UnityWebRequest.Get(ulr1))
        {
            www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");

            yield return www.SendWebRequest();
            yield return new WaitUntil(() => www.isDone);
            if (www.isHttpError)
            {
                logText.text = "isHttpError " + www.error;
            }
            else if (www.isNetworkError)
            {
                logText.text = "isNetworkError " + www.error;
            }
            else
            {
                logText.text = www.downloadHandler.text;

                if (++count > 5)
                {
                    logText.text = www.url;
                    Application.OpenURL(www.url);
                }
            }

            isProcess = false;
        }

        
    }

    private bool isDownloadAudio;

    private IEnumerator DownLoadTheAudio(AudioType audioType, string souroceText)
    {
        isDownloadAudio = true;
        this.audioType = audioType;
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=" + souroceText + "&tl=En-gb", audioType))
        {
            www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");

            yield return www.SendWebRequest();
            yield return new WaitUntil(() => www.isDone);
            if (www.isHttpError)
            {
                translateText.text = "isHttpError " + www.error;
            }
            else if (www.isNetworkError)
            {
                translateText.text = "isNetworkError " + www.error;
            }
            else
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.Play();
            }
            isDownloadAudio = false;
        }
    }

    private int count;


    #region Popup

    public GameObject dialogPanel;

    public RectTransform registedWordsParent;

    public void OnPressShowWordsButton()
    {
        dialogPanel.SetActive(true);

    }
    
    #endregion
}
