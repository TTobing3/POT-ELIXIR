using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Record
{
    public Recipe recipe;
    public List<IngredientData> ingredients;
    public int[] results;
    public string bottleType;

    public bool isSuccess
    {
        get
        {
            return results.All(result => result == 0);
        }
    }

    public Record()
    {
        recipe = new Recipe();
    }

    public Record(Recipe recipe, List<IngredientData> ingredients, int[] results, string bottleType)
    {
        this.recipe = recipe;
        this.ingredients = ingredients;
        this.results = results;
        this.bottleType = bottleType;
    }
}

public class RecordManager : MonoBehaviour
{
    public static RecordManager instance;

    public List<Record> records = new List<Record>();

    [Header("Row")]
    public GameObject recordPrefab;
    public Transform recordParents;
    public List<RecordRow> recordPool;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        SetRecord();
    }

    public void AddRecord(Record record)
    {
        records.Insert(0, record);
        if (records.Count > 40)
        {
            records.RemoveAt(records.Count - 1);
        }
    }

    public void SetRecord()
    {
        recordPool.ForEach(x => x.gameObject.SetActive(false));

        int number = 0;

        foreach(Record i in records)
        {
            if(number > 30) break;

            number++;

            RecordRow row = null;

            row = recordPool.Find(x => !x.gameObject.activeSelf);

            if(row == null)
            {
                row = Instantiate(recordPrefab, recordParents).GetComponent<RecordRow>();
                //row.transform.SetSiblingIndex(0);
                recordPool.Add(row);
            }
            
            row.gameObject.SetActive(true);
            row.Set(i);
        }

        recordParents.GetComponent<RectTransform>().DOAnchorPosY(0,0.5f); 
    }
}
