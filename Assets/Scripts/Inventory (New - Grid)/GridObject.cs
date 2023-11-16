using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GridObject
{
    public static GameObject inventoryTab;
    public static GameObject uiPrefab;
    
    Grid<GridObject> grid;
    GameObject itemSlot;
    
    public int x;
    public int y;

    public Item item;
    public Item tempItem;

    //Constructor
    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        item = null;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + item;
    }

    //changes what object placed in this grid object
    public void SetItem(Items itemName, GameObject parent)
    {
        this.item = GetItem(itemName);
        if (itemSlot == null)
        {
            itemSlot = GameObject.Instantiate(uiPrefab, new Vector3(0, 0, 0) * GridInventoryManager.instance.cellSize, Quaternion.identity, parent.transform);
        }

        Item item = GetItem(itemName);

        itemSlot.GetComponentInChildren<Image>().sprite = item.itemSprite;
        itemSlot.GetComponentInChildren<Image>().type = Image.Type.Simple;
        itemSlot.GetComponentsInChildren<RectTransform>()[1].sizeDelta = GridInventoryManager.instance.cellSize * item.itemSize;
        itemSlot.GetComponentInChildren<InteractableObject>().itemName = item.itemName;

        itemSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(GridInventoryManager.instance.cellSize, GridInventoryManager.instance.cellSize);
        itemSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0) * GridInventoryManager.instance.cellSize;

        itemSlot.GetComponentInChildren<InteractableObject>().itemPos = itemSlot.GetComponent<RectTransform>().anchoredPosition;
        
        itemSlot.SetActive(true);

        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //clear item from the gridobject
    public void ClearItem()
    {
        item = null;

        if (itemSlot != null)
        {
            itemSlot.SetActive(false);
        }

        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //checks if there is no itemscriptableobject in the gridobject
    public bool EmptyItem()
    {
        return item == null;
    }

    public void SetTemp(Items itemName)
    {
        tempItem = GetItem(itemName);
    }

    public bool EmptyTempItem()
    {
        return tempItem == null;
    }

    public void ClearTemp()
    {
        tempItem = null;
    }

    public Item GetTemp(Items itemName)
    {
        return GetItem(itemName);
    }

    public void SetTempAsReal(GameObject parent)
    {
        ClearItem();

        if (!EmptyTempItem())
        {
            SetItem(tempItem.itemName, parent);
        }

        ClearTemp();
    }

    Item GetItem(Items itemName)
    {
        List<Item> itemList = GridInventoryManager.instance.item_SO.itemList;

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemName == itemName)
            {
                return itemList[i];
            }
        }

        return null;
    }

}