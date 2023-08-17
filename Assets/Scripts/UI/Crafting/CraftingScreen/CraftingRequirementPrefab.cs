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
        craftingItemAmountDisplay.text = GetItemAmontInInventory(requirements).ToString() + "/" + requirements.amount;
    }

    public void CheckRequrement()
    {
        if (GetItemAmontInInventory(requirements) >= requirements.amount) //Enough of the Item available in Inventory
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

    int GetItemAmontInInventory(CraftingRequirements item)
    {
        int itemInventoryCounter = 0;

        for (int i = 0; i < InventorySystem.instance.inventoryItemList.Count; i++)
        {
            if (item.itemName == InventorySystem.instance.inventoryItemList[i].itemName)
            {
                itemInventoryCounter += InventorySystem.instance.inventoryItemList[i].amount;
            }
        }

        return itemInventoryCounter;
    }
}
