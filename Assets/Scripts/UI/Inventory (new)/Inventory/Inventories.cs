using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventories
{
    public bool isOpen;

    [Header("General")]
    public int index;
    public int inventorySize;

    [Header("InventoryList")]
    public List<InventoryItem> itemList = new List<InventoryItem>();
}
