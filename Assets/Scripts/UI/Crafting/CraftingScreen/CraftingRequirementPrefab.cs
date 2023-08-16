using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRequirementPrefab : MonoBehaviour
{
    [SerializeField] Image craftingItemImage;
    [SerializeField] Image craftingItemImageOverlay;
    [SerializeField] TextMeshProUGUI craftingItemName;
    [SerializeField] TextMeshProUGUI craftingItemAmountDisplay;

    [HideInInspector] public CraftingRequirements requirements;
    [HideInInspector] public Sprite craftingItemSprite;
    [HideInInspector] public int craftingItemAmountGotInInventory;

    bool requirementIsMet;


    //--------------------


    private void Start()
    {
        requirementIsMet = false;
        craftingItemImageOverlay.gameObject.SetActive(true);
        craftingItemImage.color = new Color(1, 1, 1, 0.75f);
    }


    //--------------------


    public void SetDisplay()
    {
        craftingItemImage.sprite = craftingItemSprite;
        craftingItemName.text = requirements.itemName.ToString();
        craftingItemAmountDisplay.text = craftingItemAmountGotInInventory.ToString() + "/" + requirements.amount;
    }

    public void CheckRequrement()
    {
        //if (true) //Enough of the Item available in Inventory
        //{
        //    requirementIsMet = true;
        //    craftingItemImageOverlay.gameObject.SetActive(false);
        //    craftingItemImage.color = new Color(1, 1, 1, 1);
        //}
        //else
        //{
        //    requirementIsMet = false;
        //    craftingItemImageOverlay.gameObject.SetActive(true);
        //    craftingItemImage.color = new Color(1, 1, 1, 0.75f);
        //}
    }
}
