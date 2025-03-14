using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;

[System.Serializable]
public class IngredientData
{
    public int number;
    public string name;
    public int[] roles;
    public Color color;
    
    public IngredientData()
    {
        number = 0;
        name = "";
        roles = new int[0];
    }
    public IngredientData(int number, string name, int[] roles, Color color)
    {
        this.number = number;
        this.name = name;
        this.roles = roles;
        this.color = color;
    }

}

[System.Serializable]
public class RecipeData
{
    public int number;
    public string name;
    public int size;
    public int[] ingredients;
    public int price;
    public RecipeData(int number, string name, int size, int[] ingredients, int price)
    {
        this.number = number;
        this.name = name;
        this.size = size;
        this.ingredients = ingredients;
        this.price = price;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public string[] TextData = new string[10];

    // Data

    public Dictionary<string, IngredientData> AllIngredientDatas = new Dictionary<string, IngredientData>();
    public List<IngredientData> AllIngredientDataList = new List<IngredientData>();

    public Dictionary<string, RecipeData> AllRecipeDatas = new Dictionary<string, RecipeData>();
    public List<RecipeData> AllRecipeDataList = new List<RecipeData>();

    public List<int> IngredientPriceList = new List<int>(), RecipePriceList = new List<int>(), DonatePriceList = new List<int>();
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        // IngredientData
        string[] line = TextData[0].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            int[] role = e[2].Split("/").Select(x => int.Parse(x)).ToArray();
            float[] colorArr = e[3].Split("/").Select(x => float.Parse(x)).ToArray();


            Color color = new Color(colorArr[0], colorArr[1], colorArr[2]);

            var ingredientData = new IngredientData
            (
                int.Parse(e[0]), // number
                e[1], // name
                role, // role
                color
            );

            AllIngredientDatas.Add(e[1], ingredientData);
            AllIngredientDataList.Add(ingredientData);
        }
        
        // RecipeData
        line = TextData[1].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            int[] ingredients = e[3].Split("/").Select(x => int.Parse(x)).ToArray();

            var recipeData = new RecipeData
            (
                int.Parse(e[0]), // number
                e[1], // name
                int.Parse(e[2]), // size
                ingredients, // ingredients
                int.Parse(e[4])
            );

            AllRecipeDatas.Add(e[1], recipeData);
            AllRecipeDataList.Add(recipeData);
        }

        // DonateData
        line = TextData[2].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            if(i == 1)
            {
                IngredientPriceList = e.Skip(2).Select(x => int.Parse(x)).ToList();
            }

            if(i == 2)
            {
                RecipePriceList = e.Skip(2).Select(x => int.Parse(x)).ToList();
            }

            if(i == 3)
            {
                DonatePriceList = e.Skip(2).Select(x => int.Parse(x)).ToList();
            }
        }


    }


    #region Data Load

    const string 
        ingredientURL = "https://docs.google.com/spreadsheets/d/1UwW2hoOuI99B1t78UMzbNrDGPqP-MQ8neDBpP25Iq-4/export?format=tsv&gid=0",
        recipeURL = "https://docs.google.com/spreadsheets/d/1UwW2hoOuI99B1t78UMzbNrDGPqP-MQ8neDBpP25Iq-4/export?format=tsv&gid=1770031902",
        priceURL = "https://docs.google.com/spreadsheets/d/1UwW2hoOuI99B1t78UMzbNrDGPqP-MQ8neDBpP25Iq-4/export?format=tsv&gid=554735701";
    

    
    [ContextMenu("Data Load")]
    void GetLang()
    {
        StartCoroutine(GetLangCo());
    }

    IEnumerator GetLangCo()
    {
        UnityWebRequest www = UnityWebRequest.Get(ingredientURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 0);

        www = UnityWebRequest.Get(recipeURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 1);
        
        www = UnityWebRequest.Get(priceURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 2);

        Debug.Log("Clear");
    }

    void SetDataList(string tsv, int i)
    {
        TextData[i] = tsv;
    }

    #endregion
}