using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRequirementPrefab : MonoBehaviour
{
    [SerializeField] Color overlayColor_Active;
    [SerializeField] Color overlayColor_Inactive;

    [SerializeField] Image craftingItemImage;
    [SerializeField] Image craftingItemImageOverlay;
    [SerializeField] TextMeshProUGUI craftingItemName;
    [SerializeField] TextMeshProUGUI craftingItemAmountDisplay;

    public CraftingRequirements requirements;
    [HideInInspector] public Sprite craftingItemSprite;

    public bool requirementIsMet;


    //--------------------


    private void Start()
    {
        requirementIsMet = false;
        craftingItemImageOverlay.color = new Color(overlayColor_Inactive.r, overlayColor_Inactive.g, overlayColor_Inactive.b, overlayColor_Inactive.a);
        craftingItemName.color = new Color(0.80f, 0.69f, 0.48f, 0.5f);
    }
    private void Update()
    {
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            SetDisplay();
            CheckRequrement();
        }
    }


    //--------------------


    public void SetDisplay()
    {
        craftingItemImage.sprite = craftingItemSprite;
        craftingItemName.text = requirements.itemName.ToString();
        craftingItemAmountDisplay.text = GetItemAmontInInventories(requirements).ToString() + "/" + requirements.amount;
    }

    public void CheckRequrement()
    {
        if (GetItemAmontInInventories(requirements) >= requirements.amount) //Enough of the Item available in Inventory
        {
            requirementIsMet = true;
            craftingItemImageOverlay.color = new Color(overlayColor_Active.r, overlayColor_Active.g, overlayColor_Active.b, overlayColor_Active.a);
            craftingItemImage.color = new Color(1, 1, 1, 1);
            craftingItemName.color = new Color(0.80f, 0.69f, 0.48f, 1);
        }
        else
        {
            requirementIsMet = false;
            craftingItemImageOverlay.color = new Color(overlayColor_Inactive.r, overlayColor_Inactive.g, overlayColor_Inactive.b, overlayColor_Inactive.a);
            craftingItemImage.color = new Color(1, 1, 1, 0.75f);
            craftingItemName.color = new Color(0.80f, 0.69f, 0.48f, 0.5f);
        }
    }

    int GetItemAmontInInventories(CraftingRequirements item)
    {
        int itemInventoryCounter = 0;

        List<Inventories> itemList = InventoryManager.instance.inventories;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].isOpen)
            {
                for (int j = 0; j < itemList[i].itemList.Count; j++)
                {
                    if (item.itemName == itemList[i].itemList[j].itemName)
                    {
                        //Check if item is dragging
                        if (StorageManager.instance.activeInventoryItem == itemList[i].itemList[j]
                            && StorageManager.instance.itemIsClicked)
                        {
                            itemInventoryCounter += StorageManager.instance.itemAmountSelected + StorageManager.instance.itemAmountLeftBehind;
                        }
                        else
                        {
                            itemInventoryCounter += itemList[i].itemList[j].amount;
                        }
                    }
                }
            }
        }

        return itemInventoryCounter;
    }
}
