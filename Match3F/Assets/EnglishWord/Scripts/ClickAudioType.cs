using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAudioType : MonoBehaviour
{
    public AudioType audioType;


    private TestGetAudioClip testGetAudioClip;

    private void Start()
    {
        testGetAudioClip = GetComponentInParent<TestGetAudioClip>();
    }


    public void OnClick()
    {
        testGetAudioClip.OnClick(audioType);
    }
}
