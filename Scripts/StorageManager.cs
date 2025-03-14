using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StorageManager : MonoBehaviour
{
    [Header("Tooltip")]
    public GameObject tooltipObj;
    public TextMeshProUGUI[] tooltipTexts; // name gold button
    public Button tooltipButton;

    public AudioSource[] actAudioSource;

    [Header("Storage")]
    public List<GameObject> slots;

    [Header("Selected")]
    public Record curRecord;
    public int curNumber;

    //제작 한 물약들 데이터 쌓기


    void Awake()
    {
        for(int i = 0; i<slots.Count; i++)
        {
            int n = i;
            slots[i].GetComponent<Button>().onClick.AddListener(()=>{ SetTooltip(n); });
        }

        tooltipButton.onClick.AddListener(()=>{ActTooltipButton();});

    }

    void Start()
    {
        DataCarrier.instance.addItemAction += SetSlot;
    }

    void SetTooltip(int n)
    {
        var data = DataCarrier.instance.items[n];

        if(data == null || data.recipe.name.Equals("")) return;

        tooltipObj.SetActive(true);
        tooltipObj.GetComponent<RectTransform>().anchoredPosition = slots[n].GetComponent<RectTransform>().anchoredPosition;

        // name gold button
        tooltipTexts[0].text = data.recipe.name;
        tooltipTexts[1].text = data.isSuccess ? data.recipe.recipeData.price + "$" : "-";
        tooltipTexts[2].text = data.isSuccess ? "Sell" : "Trash";

        // selected
        curNumber = n;
        curRecord = data;
    }

    void ActTooltipButton()
    {
        var data = DataCarrier.instance.items[curNumber];

        if(data.isSuccess)
        {
            print("판매");
            AudioManager.instance.PlayAudioSource(actAudioSource[0]);
            DataCarrier.instance.GainGold(data.recipe.recipeData.price);
        }
        else
        {
            print("폐기");
            AudioManager.instance.PlayAudioSource(actAudioSource[1]);
            AudioManager.instance.PlayAudioSource(actAudioSource[2]);
            EndManager.instance.GrowUp();
            DataCarrier.instance.trashCount++;
        }

        DataCarrier.instance.items[curNumber] = new Record();

        SetSlot();
    }

    public void SetSlot()
    {
        for(int i = 0; i<DataCarrier.instance.items.Length; i++)
        {
            var item = DataCarrier.instance.items[i];

            int number = i ;

            if(item == null || item.recipe.name.Equals(""))
            {
                slots[number].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                slots[number].transform.GetChild(0).gameObject.SetActive(true);
                slots[number].transform.GetChild(0).GetComponent<BottleImage>().SetBottle(item.bottleType);
                slots[number].GetComponentsInChildren<Image>().First(x=>x.gameObject.name == "Liquid").color = item.recipe.color;
            }
        }
    }

    
    Color RandomColor()
    {
        return new Color( Random.Range(0.3f, 0.8f), Random.Range(0.3f, 0.8f), Random.Range(0.3f, 0.8f), 1 );
    }
}
