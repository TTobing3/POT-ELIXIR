using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering.Universal;
public class EndManager : MonoBehaviour
{
    public static EndManager instance;
    public Transform monster;

    [Header("Camera")]
    public Transform mainCamera;

    [Header("Light")]
    public Light2D light2D;
    public Light2D purpleLight2D, monsterLight2D;

    [Header("UI")]
    public CanvasGroup screenCanvasGroup;
    public GameObject endScreen;
    public TextMeshProUGUI endText;
    
    public GameObject elixirEnd;
    public Transform elixirTransform;

    [Header("Audio")]
    public AudioClip endClip;
    public AudioSource elixirAudioSource, monsterAudioSource;
    public AudioClip[] monsterClips;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void GrowUp()
    {
        monster.localScale = new Vector3(monster.localScale.x + 0.01f, monster.localScale.y + 0.01f, 1);
    }

    [ContextMenu("엘릭서 엔딩")]
    public void ElixirEnd()
    {
        // 엘릭서 등장
        elixirEnd.SetActive(true);

        AudioManager.instance.StopBGM();

        screenCanvasGroup.DOFade(0, 1f).OnComplete(()=>
        {
            screenCanvasGroup.gameObject.SetActive(false);
            elixirTransform.gameObject.SetActive(true);
            elixirTransform.localScale = Vector2.zero;
            purpleLight2D.intensity = 0;
            DOTween.To(() => purpleLight2D.intensity, x => purpleLight2D.intensity = x, 5f, 2f);

            AudioManager.instance.PlayAudioSource(elixirAudioSource);
        });
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0.1f, 2f).SetDelay(1f);

        elixirTransform.DOScale(new Vector3(4,4),3).SetEase(Ease.OutBack).SetDelay(2);
        endScreen.gameObject.SetActive(true);
        EndText("You have successfully completed the Elixir...",()=>{ EndText("But...",ShowMonster);});
    }

    public void EndText(string text, System.Action action)
    {
        endText.DOFade(0,1).OnComplete(()=>
        {
            endText.text = "";
            endText.color = Color.white;
            endText.DOText(text, 3).OnComplete(()=>
            {
                if(action != null) action();
            });
        });
    }

    public void ShowMonster()
    {
        AudioManager.instance.StartBGM(endClip);
        EndText("The trashed potion has created Something...", null);

        mainCamera.DOMoveX(monster.position.x, 5).SetEase(Ease.OutBack).OnComplete(()=>{ActMonster();});

    }

    public void ActMonster()
    {
        StartCoroutine(PlayMonsterSounds());

        IEnumerator PlayMonsterSounds()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(6f, 8f));
                AudioClip randomClip = monsterClips[Random.Range(0, monsterClips.Length)];
                monsterAudioSource.clip = randomClip;
                AudioManager.instance.PlayAudioSource(monsterAudioSource);
            }
        }

        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0f, 2f).SetDelay(3f);
        DOTween.To(() => monsterLight2D.intensity, x => monsterLight2D.intensity = x, 0.1f, 4f).OnComplete(()=>
        {
            monster.DOScaleX(4,5).SetEase(Ease.InOutBounce);
            monster.DOScaleY(4,5).SetEase(Ease.InOutCirc);
        });

        string donateString = DataCarrier.instance.donate > 10 ? ":D" : ":)";
        string quick = DataCarrier.instance.quick ? "Quick" : "";

        if(DataCarrier.instance.donate > 50) donateString = ":O"; 

        string recordString = $"[ Your {quick} Record ]\n"+$"Total Try : {DataCarrier.instance.totalCount}\n"+
        $"Perfect Potion : {DataCarrier.instance.successCount}\n"+
        $"Trashed Potion : {DataCarrier.instance.trashCount}\n"+
        $"Discovered Potions : {DataCarrier.instance.successPotionCount+1} / 22\n"+
        $"Donate Count {donateString} : {DataCarrier.instance.donate}\n";


        endText.DOFade(0,1).SetDelay(5).OnComplete(()=>
        {
            endText.text = "";
            endText.color = Color.white;
            endText.DOText(recordString, 10);
        });
    }
}
