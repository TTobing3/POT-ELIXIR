using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;

public class SlotImageAdapter : MonoBehaviour
{
    Image image;
    SpriteRenderer spriteRenderer;
    SpriteResolver spriteResolver;
    RectTransform rect;

    string category, item;

    Color color = Color.white;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        spriteResolver = GetComponent<SpriteResolver>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ImageChange(string category, string item, bool resize = true, bool offImage = true)
    {
        if (item == "None") gameObject.SetActive(false);
        if (image != null && offImage) image.enabled = false;
        
        this.category = category;
        this.item = item;

        spriteRenderer.sprite = null;
        spriteResolver.SetCategoryAndLabel(category, item);

        StartCoroutine(CoSpriteChange(resize));
    }

    IEnumerator CoSpriteChange(bool resize = true)
    {
        yield return null;

        if(image != null)
        {
            image.enabled = true;
            image.sprite = spriteRenderer.sprite;

            if (resize) image.SetNativeSize();
        }
    }

}
