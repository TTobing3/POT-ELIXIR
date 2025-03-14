using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class Recipe
{
    public string name;
    public RecipeData recipeData;
    public int[] recipe;
    public Color color;
    public string bottleType;
    public bool isSuccess = false;

    public Recipe()
    {
        name = "";
    }

    public Recipe(string name, RecipeData data, int[] ingredients, Color color, string bottleType)
    {
        this.name = name;
        recipeData = data;
        recipe = ingredients;
        this.color = color;
        this.bottleType = bottleType;
    }
}

public class CraftManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI targetText;
    public Button craftButton;
    public Transform selectedTransform;
    public Transform potionTranform;
    public Transform ingredientsTranform;

    [Header("Audio")]
    public AudioSource ingredientAudioSource;
    public AudioSource ingredientBottleAudioSource;
    public AudioSource[] potionResultAudioSources;

    [Header("Screen")]
    public GameObject craftScreen;
    public GameObject recordScreen, ElixirScreen;

    [Header("Potion")]
    public RectTransform potionLiquidRect;
    public RectTransform potionBottleRect;

    [Header("Recipe")]
    public GameObject checkMark;
    public RecipeData curRecipeData;
    public int[] curRecipe;
    public Color curColor;
    public string curBottleType;
    public int curNumber;

    public List<Recipe> recipeList = new List<Recipe>();

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
        SetRecipe(0);
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

        selectedIngredients = new IngredientData[curRecipeData.size];

        DOVirtual.DelayedCall(0.1f, () => SetLiquid());
    }

    // 레시피 세팅
    public void SetRecipe(int n)
    {
        curRecipeData = DataManager.instance.AllRecipeDataList[n];

        selectedIngredients = new IngredientData[curRecipeData.size];

        targetText.text = curRecipeData.name;

        potionLiquidRect.GetComponent<Image>().DOColor(new Color(0.5f, 0.8f, 0.9f), 0.1f);

        // selected

        selectedPool.ForEach(x => 
        {
            x.SetActive(false);
            x.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
        });

        for(int i = 0; i<curRecipeData.size; i++)
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

        if(n < recipeList.Count)
        {
            curRecipe = recipeList[n].recipe;
            curBottleType = recipeList[n].bottleType;
            potionBottleRect.GetComponent<BottleImage>().SetBottle(curBottleType);

            
        }
        else
        {
            // 레시피피

            curRecipe = curRecipeData.ingredients.OrderBy(x => Random.value).Take(curRecipeData.size).ToArray();
            
            // bottle
            
            string[] sizeValue = new string[]{"S", "M", "L", "XL"};
            int bottleType = curRecipeData.size == 5 ?  Random.Range(0,2) : Random.Range(0,3);

            curBottleType = sizeValue[curRecipeData.size-2]+bottleType;

            potionBottleRect.GetComponent<BottleImage>().SetBottle(curBottleType);

            //

            var recipe = new Recipe(curRecipeData.name, curRecipeData, curRecipe, curColor, curBottleType);
            
            recipeList.Add(recipe);
        }
        
        if(recipeList[n].isSuccess) checkMark.SetActive(true);
        else checkMark.SetActive(false);
    }
    
    
    [ContextMenu("재료 세팅")]
    // 사용 재료 세팅
    public void SetIngredients()
    {
        if(!craftScreen.activeSelf) return;

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

    #region Interect

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

            if(result[0] == curRecipe.Length)
            {
                AudioManager.instance.PlayAudioSource(potionResultAudioSources[0]);

                DataCarrier.instance.successCount++;

                print("제조 성공");
                print(recipeList[curNumber].recipeData.name);

                
                if(!recipeList[curNumber].isSuccess)
                {
                    recipeList[curNumber].isSuccess = true;
                    checkMark.SetActive(true);
                    DataCarrier.instance.successPotionCount++;
                }




                switch(recipeList[curNumber].recipeData.name)
                {
                    case "Dragon Knight" :
                    if(DataCarrier.instance.addIngredients.Contains("Crystal of Life")) break;
                    DataCarrier.instance.addIngredients.Add("Crystal of Life");
                    UIManager.instance.SetPopUp("New ingredient obtained: Crystal of Life!");
                    SetIngredients();
                    break;
                    case "Love" :
                    if(DataCarrier.instance.addIngredients.Contains("Heart")) break;
                    DataCarrier.instance.addIngredients.Add("Heart");
                    UIManager.instance.SetPopUp("New ingredient obtained: Heart!");
                    SetIngredients();
                    break;
                    case "Reflect" :
                    if(DataCarrier.instance.addIngredients.Contains("Shard of Time")) break;
                    DataCarrier.instance.addIngredients.Add("Shard of Time");
                    UIManager.instance.SetPopUp("New ingredient obtained: Shard of Time!");
                    SetIngredients();
                    break;
                    case "Holy" :
                    if(DataCarrier.instance.addIngredients.Contains("Tears of God")) break;
                    DataCarrier.instance.addIngredients.Add("Tears of God");
                    UIManager.instance.SetPopUp("New ingredient obtained: Tears of God!");
                    SetIngredients();
                    break;
                    case "Mind Control" :
                    if(DataCarrier.instance.addIngredients.Contains("Over Mind")) break;
                    DataCarrier.instance.addIngredients.Add("Over Mind");
                    UIManager.instance.SetPopUp("New ingredient obtained: Over Mind!");
                    SetIngredients();
                    break;
                    case "Illusion" :
                    if(DataCarrier.instance.addIngredients.Contains("Hide")) break;
                    DataCarrier.instance.addIngredients.Add("Hide");
                    UIManager.instance.SetPopUp("New ingredient obtained: Hide!");
                    SetIngredients();
                    break;
                    case "Divine" :
                    if(DataCarrier.instance.addIngredients.Contains("Angel")) break;
                    DataCarrier.instance.addIngredients.Add("Angel");
                    UIManager.instance.SetPopUp("New ingredient obtained: Angel!");
                    SetIngredients();
                    break;
                }
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

        return count == curRecipeData.size;
    }

    int[] CheckIngredients()
    {
        int[] checkList = new int[3]{0, 0, 0};
        int[] results = new int[curRecipe.Length];

        for(int i = 0; i<curRecipe.Length;i++)
        {
            var color = new Color(0, 0, 0, 0.5f);

            if(selectedIngredients[i].roles.Contains(curRecipe[i]))
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
                    if(curRecipe.Contains(j))
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

        recipeList[curNumber].color = curColor;

        var record = new Record
        (
            recipeList[curNumber],
            selectedIngredients.ToList(),
            results,
            curBottleType
        );

        RecordManager.instance.AddRecord(record);
        DataCarrier.instance.AddItem(record);

        return checkList;
    }

    #endregion

    #region Recipe

    public void NextRecipe(bool isNext)
    {
        if(isNext)
        {
            if(curNumber == DataManager.instance.AllRecipeDataList.Count-2 || curNumber == DataCarrier.instance.recipe) return;
            else curNumber = curNumber + 1; 

            
        }
        else if(!isNext)
        {
            if(curNumber == 0) return;
            else curNumber = curNumber - 1; 
        }

        SetRecipe(curNumber);
        Reset();
    }

    #endregion

    #region UI

    void ShakeBottle()
    {
        DOTween.Kill(potionBottleRect);

        potionBottleRect.DORotate(new Vector3(0,0,-20),0.5f).OnComplete(()=>
        {
            potionBottleRect.DORotate(new Vector3(0,0,20),0.5f).OnComplete(()=>
            {
                potionBottleRect.DORotate(new Vector3(0,0,0),0.5f);
            });
        });
    }

    void SetLiquid()
    {
        // Rect
        float bottleHeight = potionBottleRect.sizeDelta.y;

        float size = (bottleHeight - 10) / curRecipe.Length;

        float count = selectedIngredients.Count(x=> x != null && !x.number.Equals(0));

        if(count == curRecipe.Length)
        {
            DOTween.To(() => potionLiquidRect.offsetMax, x => potionLiquidRect.offsetMax = x, Vector2.zero, 0.5f);
        }
        else
        {
            DOTween.To(() => potionLiquidRect.offsetMax, x => potionLiquidRect.offsetMax = x, new Vector2(0, -(bottleHeight - size * count )), 0.5f);
        }

        // Color
        if(count != 0)
        {
            Color mixedColor = Color.black;
            float totalWeight = 0f;

            for (int i = 0; i < selectedIngredients.Length; i++)
            {
                var ingredient = selectedIngredients[i];
                if (ingredient != null && ingredient.number != 0)
                {
                    float weight = (i + 1) / (float)selectedIngredients.Length;
                    mixedColor += ingredient.color * weight;
                    totalWeight += weight;
                }
            }

            if (totalWeight > 0)
            {
                mixedColor /= totalWeight;
            }

            potionLiquidRect.GetComponent<Image>().DOColor(mixedColor, 0.5f);


            curColor = mixedColor;
        }
    }

    Color RandomColor()
    {
        return new Color( Random.Range(0.3f, 0.8f), Random.Range(0.3f, 0.8f), Random.Range(0.3f, 0.8f), 1 );
    }

    #endregion

}
