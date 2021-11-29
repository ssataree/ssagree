using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

public class TestGetAudioClip : MonoBehaviour
{
    public Text logText;

    public AudioSource audioSource;

    public AudioType audioType;



    private bool isClick;

    public void OnClick(AudioType audioType)
    {
        if (isClick)
            return;

        StartCoroutine(DownLoadTheAudio(audioType));
        
    }

    public void ClearLog()
    {
        logText.text = string.Empty;
        count = 0;
    }

    public void GetExcel()
    {
        if (isClick)
            return;

        StartCoroutine(DownLoadTheExcel());
    }

    private IEnumerator DownLoadTheExcel()
    {
        isClick = true;
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

            isClick = false;
        }

        
    }

    private IEnumerator DownLoadTheAudio(AudioType audioType)
    {
        isClick = true;
        this.audioType = audioType;
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=SampleTet&tl=En-gb", audioType))
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
                logText.text = www.url;
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.Play();
                if(++count > 5)
                {
                    logText.text = www.url;
                    Application.OpenURL(www.url);
                }
            }
            isClick = false;
        }
    }

    private int count;
}
