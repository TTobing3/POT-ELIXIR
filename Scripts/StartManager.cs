using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering.Universal;

public class StartManager : MonoBehaviour
{
    public Light2D light2D;
    public Light2D purpleLight2D;
    public List<Light2D> eyeLights;
    public CanvasGroup screenCanvasGroup;

    public List<TextMeshPro> texts;

    public GameObject startParentsObj, guide;

    [ContextMenu("시작")]
    public void GameStart()
    {
        texts.ForEach(x=>x.DOFade(0,2));

        DOTween.To(() => purpleLight2D.intensity, x => purpleLight2D.intensity = x, 0f, 1f).SetDelay(2f);

        eyeLights.ForEach( i=>DOTween.To(() => i.intensity, x => i.intensity = x, 0f, 1f));

        DOVirtual.DelayedCall(3.5f, ()=>
        {
            DOTween.To(() => eyeLights[0].intensity, x => eyeLights[0].intensity = x, 10f, 0.5f).OnComplete(()=>
            {
                DOTween.To(() => eyeLights[0].intensity, x => eyeLights[0].intensity = x, 0f, 1f);
            });

            
            DOTween.To(() => eyeLights[1].intensity, x => eyeLights[1].intensity = x, 10f, 0.5f).OnComplete(()=>
            {
                DOTween.To(() => eyeLights[1].intensity, x => eyeLights[1].intensity = x, 0f, 1f);
            });
        });

        DOVirtual.DelayedCall(5, SetGameScreen);
    }

    void SetGameScreen()
    {
        startParentsObj.SetActive(false);
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 1f, 2f).OnComplete(()=>
        {
            screenCanvasGroup.DOFade(1,2).OnComplete(()=>
            {
                DOVirtual.DelayedCall(1,()=>{guide.SetActive(true);} );
            });
        });
    }
}
