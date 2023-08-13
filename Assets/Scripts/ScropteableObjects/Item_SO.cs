using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Sprite itemSprite;
    [TextArea (5, 10)] public string itemDescription;

    [Header("Stats")]
    public int itemIndex;
    public int itemStackMax;
}