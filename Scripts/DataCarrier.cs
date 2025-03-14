using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using TMPro;

public class DataCarrier : MonoBehaviour
{
    public System.Action addItemAction, addIngredientAction;

    public static DataCarrier instance;

    public bool quick;

    public int[] limit = new int[2]; // 재료, 레시피피

    public int recipe = 0;
    public int ingredient = 0;
    public int gold = 0;
    public int donate = 0, trash = 0;

    public List<string> addIngredients = new List<string>();

    [Header("Record")]
    public int totalCount = 0;
    public int successCount = 0, trashCount = 0;
    public int successPotionCount = 0;


    [Header("Audio")]
    public AudioSource ingredientAudioSource;
    public AudioSource recipeAudioSource, donateAudioSource;

    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI[] shopTexts;
    public TextMeshProUGUI[] shopPriceTexts;

    public Record[] items = new Record[9];

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        shopPriceTexts[0].text = DataManager.instance.IngredientPriceList[0]+"$";
        shopPriceTexts[1].text = DataManager.instance.RecipePriceList[0]+"$";
        shopPriceTexts[2].text = DataManager.instance.DonatePriceList[0]+"$";
    }

    public void AddItem(Record record)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null || items[i].recipe.name.Equals(""))
            {
                items[i] = record;
                break;
            }
        }

        if(addItemAction != null) addItemAction();
    }

    public void GainGold(int gold)
    {
        this.gold += gold;

        SetGoldText();
    }
    public bool UseGold(int gold)
    {
        if(this.gold < gold) 
        {
            UIManager.instance.SetPopUp("Not Enough Gold");
            return false;
        }
        else
        {
            this.gold -= gold;
            SetGoldText();
            return true;
        }
    }
    public void SetGoldText()
    {
        goldText.DOText(gold+"$",0.5f);
    }

    public void AddIngredient()
    {
        if(ingredient == limit[0]) 
        {
            UIManager.instance.SetPopUp("All Ingredients Purchased");
            return;
        }

        print(gold);
        print(DataManager.instance.IngredientPriceList[ingredient-3]);

        if(gold < DataManager.instance.IngredientPriceList[ingredient-3])
        {
            UIManager.instance.SetPopUp("Not Enough Gold");
            return;
        }

        UseGold(DataManager.instance.IngredientPriceList[ingredient-3]);
        ingredient++;

        AudioManager.instance.PlayAudioSource(ingredientAudioSource);

        if(ingredient == limit[0]) 
        {
            shopTexts[0].text = $"M\nIngredient";
            shopPriceTexts[0].text = "-$";
        }
        else
        {
            shopTexts[0].text = $"{ingredient-3}\nIngredient";
            shopPriceTexts[0].text = DataManager.instance.IngredientPriceList[ingredient-3]+"$";
        }

        addIngredientAction();
    }

    public void AddRecipe()
    {
        if(recipe == limit[1]) 
        {
            UIManager.instance.SetPopUp("All Recipe Purchased");
            return;
        }
        
        if(gold < DataManager.instance.RecipePriceList[recipe])
        {
            UIManager.instance.SetPopUp("Not Enough Gold");
            return;
        }

        UseGold(DataManager.instance.RecipePriceList[recipe]);
        recipe++;
        
        AudioManager.instance.PlayAudioSource(recipeAudioSource);

        if(recipe == limit[1]) 
        {
            shopTexts[1].text = $"M\nRecipe";
            shopPriceTexts[1].text = "-$";
        }
        else
        {
            shopTexts[1].text = $"{recipe}\nRecipe";
            shopPriceTexts[1].text = DataManager.instance.RecipePriceList[recipe]+"$";
            shopPriceTexts[2].text = DataManager.instance.RecipePriceList[recipe]*2+"$";
        }
    }

    public void Donate()
    {
        if(gold < DataManager.instance.RecipePriceList[recipe]*2)
        {
            UIManager.instance.SetPopUp("Not Enough Gold");
            return;
        }

        UseGold(DataManager.instance.RecipePriceList[recipe]*2);
        donate++;
        
        AudioManager.instance.PlayAudioSource(donateAudioSource);
        
        if(donate == limit[2]) 
        {
            shopTexts[2].text = $"M\nDonate";
            shopPriceTexts[2].text = "-$";
        }
        else
        {
            shopTexts[2].text = $"{donate}\nDonate";
            shopPriceTexts[2].text = DataManager.instance.RecipePriceList[recipe]*2+"$";
        }
    }

    public void AddTrash()
    {
        trash++;

        if(trash > 250)
        {
            print("배드엔딩");
        }
    }

    public void OnQuickMode()
    {
        print("Quick");
        quick = true;
        
        ingredient = limit[0];

        for(int i = limit[0]; i<DataManager.instance.AllIngredientDataList.Count; i++)
        {
            addIngredients.Add(DataManager.instance.AllIngredientDataList[i].name);
        }
        
        addIngredientAction();
    }

}
