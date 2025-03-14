using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordRow : MonoBehaviour
{
    public Record record;

    public TextMeshProUGUI nameText;
    public Image liquid;
    public BottleImage bottleImage;

    public List<Transform> slots = new List<Transform>();

    void Start()
    {
        print("Row");
        print(gameObject.name);
    }

    public void Set(Record record)
    {
        this.record = record;

        slots.ForEach(x => x.gameObject.SetActive(false));

        var colors = new Color[3]{ Color.green, Color.yellow, Color.red };

        for(int i = 0; i<record.ingredients.Count; i++)
        {
            slots[i].gameObject.SetActive(true);

            slots[i].GetChild(0).GetComponent<SlotImageAdapter>().ImageChange("Ingredient", record.ingredients[i].name);
            slots[i].GetChild(1).GetComponent<Image>().color = colors[ record.results[i] ];
        }

        nameText.text = record.recipe.name;
        liquid.color = record.recipe.color;
        bottleImage.SetBottle(record.bottleType);
    }
}
