using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public AudioSource uiAudioSource;

    [Header("PopUp")]
    public AudioSource popUpAudioSource;
    public GameObject popUpObj;
    public TextMeshProUGUI popUpText;

    Tween popUpTween = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void SetQuickPopUp()
    {
        DataCarrier.instance.OnQuickMode();
    }

    public void SetPopUp(string text)
    {
        if(popUpTween != null) popUpTween.Kill();

        popUpObj.SetActive(true);
        popUpText.text = text;

        AudioManager.instance.PlayAudioSource(popUpAudioSource);

        popUpTween = DOVirtual.DelayedCall(1, ()=>
        {
            popUpObj.SetActive(false);
        });
    }

    public void PlayUISfx()
    {
        AudioManager.instance.PlayAudioSource(uiAudioSource);
    }

    public void PlayUISfx(AudioSource audioSource)
    {
        AudioManager.instance.PlayAudioSource(audioSource);
    }
}
