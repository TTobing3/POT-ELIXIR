using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;

public class BottleImage : MonoBehaviour
{
    public Image[] images;

    public void SetBottle(string name)
    {
        int j = 0;

        foreach(SpriteResolver i in GetComponentsInChildren<SpriteResolver>())
        {
            if(i.GetCategory() == "Bottle")
            {
                i.GetComponent<SlotImageAdapter>().ImageChange("Bottle", name, j == 0, false);
            }
            else
            {
                i.GetComponent<SlotImageAdapter>().ImageChange("Base", name, j == 0, false);
            }

            j++;

        }
    }
}
