using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionSubButtonPrefab : MonoBehaviour
{
    [SerializeField] Image buttonImage;

    public Item item = new Item();

    public void SetDisplay()
    {
        buttonImage.sprite = item.itemSprite;
        gameObject.name = item.itemName.ToString();
    }

    public void Button_OnClick()
    {
        CraftingManager.instance.itemSelected = item;
    }
}
