using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class Item_SO : ScriptableObject
{
    public List<Item> itemList = new List<Item>();
}

[Serializable]
public class Item
{
    [Header("General")]
    public Items itemName;
    public ItemCategories categoryName;
    public ItemSubCategories subCategoryName;
    public Sprite itemSprite;
    [TextArea (5, 10)] public string itemDescription;

    [Header("Stats")]
    public bool isActive;
    public int itemStackMax;

    [Header("Crafting")]
    public bool isCrafteable = false;
    public List<CraftingRequirements> craftingRequirements = new List<CraftingRequirements>();

    
}

[Serializable]
public class CraftingRequirements
{
    public Items itemName;
    public int amount;
}