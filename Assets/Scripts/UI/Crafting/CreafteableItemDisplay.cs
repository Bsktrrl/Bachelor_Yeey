using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreafteableItemDisplay : MonoBehaviour
{
    [Header("ItemDisplay Info")]
    public string itemDisplayName;
    public Sprite itemDisplaySprite;

    [Header("SerializeFields")]
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public Button craftingButton;
    public TextMeshProUGUI requirementText;

    [Header("Requirement Materials")]
    public List<Requirement> requirement;


    //--------------------


    private void Start()
    {
        itemNameText.text = itemDisplayName;
        itemImage.sprite = itemDisplaySprite;

        requirementText.text = "";
        foreach (Requirement item in requirement)
        {
            requirementText.text += item.itemAmount + " " + item.itemName + "[" + "0" + "]" + "\n"; //Swap "0" with current holding number
        }
    }


    //--------------------


    public void CraftAnyItem()
    {
        //Remove items from inventory
        //InventorySystem.instance.RemoveItemFromInventory(requirement);

        //Add item to inventory
        //InventorySystem.instance.AddtoInventory();

        //Refresh List
        //InventorySystem.instance.RecalculateList();

        //Refresh requiredItems
        RefreshRequiredItems();

    }


    //--------------------


    void RefreshRequiredItems()
    {

    }
}


