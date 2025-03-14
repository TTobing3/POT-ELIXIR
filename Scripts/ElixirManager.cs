using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ElixirManager : MonoBehaviour
{
    [Header("UI")]
    public Button craftButton;
    
    [Header("Audio")]
    public AudioSource ingredientAudioSource;
    public AudioSource ingredientBottleAudioSource;
    public AudioSource[] potionResultAudioSources;

    [Header("Screen")]
    public GameObject craftScreen;
    public GameObject recordScreen, elixirScreen;

    [Header("Potion")]
    public RectTransform potionLiquidRect;
    public RectTransform potionBottleRect;

    [Header("Recipe")]
    public RecipeData elixirRecipeData;
    public Recipe elixirRecipe;
    bool isSet = false;
    
    [Header("SelectPool")]
    public GameObject selectedPrefab;
    public Transform selectedParents;
    public List<GameObject> selectedPool;
    
    [Header("IngredientPool")]
    public int ingredientCount;
    public GameObject ingredientPrefab;
    public Transform ingredientsParents;
    public List<GameObject> ingredientsPool;
    
    [Header("Datas")]
    [SerializeField] IngredientData[] selectedIngredients;
    [SerializeField] List<IngredientData> ingredients = new List<IngredientData>();

    void Awake()
    {
        craftButton.onClick.AddListener(CraftPotion);
    }

    void Start()
    {
        DataCarrier.instance.addIngredientAction += SetIngredients;

        Set();   
    }
    #region Setting

    [ContextMenu("세팅")]
    public void Set()
    {
        SetRecipe();
        SetIngredients();
        
        Reset();
    }

    void Reset()
    {
        selectedPool.ForEach(x => 
        {
            x.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
            x.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        });

        selectedIngredients = new IngredientData[elixirRecipeData.size];

        DOVirtual.DelayedCall(0.1f, () => SetLiquid());
    }

    // 레시피 세팅
    public void SetRecipe()
    {
        elixirRecipeData = DataManager.instance.AllRecipeDataList[DataManager.instance.AllRecipeDataList.Count-1];

        selectedIngredients = new IngredientData[elixirRecipeData.size];

        // selected

        selectedPool.ForEach(x => 
        {
            x.SetActive(false);
            x.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
        });

        for(int i = 0; i<elixirRecipeData.size; i++)
        {
            // set obj
            GameObject selectedObj = null;

            selectedObj = selectedPool.Find(x => x.activeSelf == false);

            if(selectedObj == null)
            {
                selectedObj = Instantiate(selectedPrefab, selectedParents);
                selectedPool.Add(selectedObj);
            }

            selectedObj.SetActive(true);

            int number = i;

            selectedObj.GetComponent<Button>().onClick.AddListener(()=>{CancleIngredient(number);});
            selectedObj.GetComponent<Image>().color = new Color(0,0,0, 0.5f);
        }

        if(isSet)
        {

        }
        else
        {
            var curRecipe = elixirRecipeData.ingredients.OrderBy(x => Random.value).Take(elixirRecipeData.size).ToArray();

            elixirRecipe = new Recipe(elixirRecipeData.name, elixirRecipeData, curRecipe, new Color(0.6f, 0.35f, 0.75f), "E0");

            isSet = true;
        }
    }
    
    
    [ContextMenu("재료 세팅")]
    // 사용 재료 세팅
    public void SetIngredients()
    {
        if(!elixirScreen.activeSelf) return;

        ingredients = new List<IngredientData>();
        ingredientsPool.ForEach(x => x.SetActive(false));

        for(int i = 0; i<DataCarrier.instance.ingredient; i++)
        {
            // set data

            var ingredientData = DataManager.instance.AllIngredientDataList[i];

            
            int number = i;

            AddIndredient(ingredientData, number);
        }

        for(int i = 0; i<DataCarrier.instance.addIngredients.Count; i++)
        {
            var ingredientData = DataManager.instance.AllIngredientDatas[DataCarrier.instance.addIngredients[i]];

            
            int number = DataCarrier.instance.ingredient + i;

            AddIndredient(ingredientData, number);
        }
    }

    void AddIndredient(IngredientData ingredientData, int number)
    {
        ingredients.Add( ingredientData );

            // set obj
            GameObject ingredientObj = null;

            ingredientObj = ingredientsPool.Find(x => x.activeSelf == false);

            if(ingredientObj == null)
            {
                ingredientObj = Instantiate(ingredientPrefab, ingredientsParents);
                ingredientsPool.Add(ingredientObj);
            }

            ingredientObj.SetActive(true);


            ingredientObj.GetComponentInChildren<SlotImageAdapter>().ImageChange("Ingredient", ingredientData.name);
            ingredientObj.GetComponent<Button>().onClick.RemoveAllListeners();
            ingredientObj.GetComponent<Button>().onClick.AddListener(()=>{SelectIngredient(number);});
    }

    #endregion

    // 재료 선택
    public void SelectIngredient(int number)
    {
        print("select!");
        if (selectedIngredients.All(x => x != null && x.number != 0))
        {
            UIManager.instance.SetPopUp("Cannot Select Anymore");
            return;
        } 

        var targetIngredient = ingredients[number];
        
        if(selectedIngredients.Contains(targetIngredient))
        {
            UIManager.instance.SetPopUp("Already Selected");
            return;
        } 
        AudioManager.instance.PlayAudioSource(ingredientAudioSource);
        AudioManager.instance.PlayAudioSource(ingredientBottleAudioSource);

        for (int i = 0; i < selectedIngredients.Length; i++)
        {
            if (selectedIngredients[i] == null || selectedIngredients[i].number == 0)
            {
                selectedIngredients[i] = targetIngredient;

                var selectedIngredientsImage = selectedPool[i].transform.GetChild(0).GetComponent<Image>();
                selectedIngredientsImage.color = Color.white;

                selectedIngredientsImage.GetComponentInChildren<SlotImageAdapter>().ImageChange("Ingredient", targetIngredient.name);
                break;
            }
        }
        
        SetLiquid();
    }
    // 선택 취소
    public void CancleIngredient(int number)
    {
        AudioManager.instance.PlayAudioSource(ingredientAudioSource);
        selectedIngredients[number] = new IngredientData();
        
        var selectedIngredientsImage = selectedPool[number].transform.GetChild(0).GetComponent<Image>();
        selectedIngredientsImage.color = Color.clear;

        selectedPool[number].GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        SetLiquid();
    }
    
    public void CraftPotion()
    {
        if(!CheckCount())
        {
            UIManager.instance.SetPopUp("Not Enough Ingredients");
        }
        else if(!DataCarrier.instance.items.ToList().Any(x=>x == null || x.recipe.name.Equals("")))
        {
            UIManager.instance.SetPopUp("Not Enough Space\n( Sell or Trash )");
        }
        else
        {
            
            DataCarrier.instance.totalCount++;

            int[] result = CheckIngredients();
            
            ShakeBottle();

            if(result[0] == elixirRecipeData.size)
            {
                print("엘릭서 제조 성공");

                DataCarrier.instance.successCount++;

                AudioManager.instance.PlayAudioSource(potionResultAudioSources[0]);
                elixirRecipe.isSuccess = true;

                EndManager.instance.ElixirEnd();
            }
            else
            {
                AudioManager.instance.PlayAudioSource(potionResultAudioSources[1]);
                print($"제조 실패 / 일치 {result[0]}, 포함 {result[1]}, 미포함 {result[2]}");

            }
            
            //Reset();
        }
    }

    bool CheckCount()
    {
        int count = selectedIngredients.Count(x => { return (x != null && x.number != 0); });

        return count == elixirRecipeData.size;
    }
    int[] CheckIngredients()
    {
        int[] checkList = new int[3]{0, 0, 0};
        int[] results = new int[elixirRecipeData.size];

        for(int i = 0; i<elixirRecipeData.size;i++)
        {
            var color = new Color(0, 0, 0, 0.5f);

            if(selectedIngredients[i].roles.Contains(elixirRecipe.recipe[i]))
            {
                checkList[0]++;
                results[i] = 0;
                color = new Color(0, 1, 0, 0.5f);
            }
            else
            {
                bool isContains = false;

                foreach(int j in selectedIngredients[i].roles)
                {
                    if(elixirRecipe.recipe.Contains(j))
                    {
                        checkList[1]++;
                        results[i] = 1;
                        color = new Color(1, 1, 0, 0.5f);
                        isContains = true;
                        break;
                    }
                }

                if(!isContains)
                {
                    color = new Color(1, 0, 0, 0.5f);
                    checkList[2]++;
                    results[i] = 2;
                }
            }

            selectedPool[i].GetComponent<Image>().color = color;
            selectedPool[i].transform.GetChild(1).GetComponent<Image>().color = color;

        }

        var record = new Record
        (
            elixirRecipe,
            selectedIngredients.ToList(),
            results,
            "E0"
        );

        RecordManager.instance.AddRecord(record);
        DataCarrier.instance.AddItem(record);

        return checkList;
    }
    void ShakeBottle()
    {
        DOTween.Kill(potionBottleRect);

        potionBottleRect.DOAnchorPosY(potionBottleRect.anchoredPosition.y -10, 0.5f).OnComplete(()=>
        {
            potionBottleRect.DOAnchorPosY(potionBottleRect.anchoredPosition.y +20, 0.5f).OnComplete(()=>
            {
                potionBottleRect.DOAnchorPosY(potionBottleRect.anchoredPosition.y -10, 0.5f);
            });
        });
    }
    void SetLiquid()
    {
        // Rect
        float bottleHeight = potionBottleRect.sizeDelta.y;

        float size = (bottleHeight - 10) / elixirRecipeData.size;

        float count = selectedIngredients.Count(x=> x != null && !x.number.Equals(0));

        if(count == elixirRecipeData.size)
        {
            DOTween.To(() => potionLiquidRect.offsetMax, x => potionLiquidRect.offsetMax = x, Vector2.zero, 0.5f);
        }
        else
        {
            DOTween.To(() => potionLiquidRect.offsetMax, x => potionLiquidRect.offsetMax = x, new Vector2(0, -(bottleHeight - size * count )), 0.5f);
        }

    }
}
