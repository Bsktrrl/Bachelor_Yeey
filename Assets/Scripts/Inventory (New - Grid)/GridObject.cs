using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridObject
{
    public static GameObject inventoryTab;
    public static GameObject uiPrefab;
    private Grid<GridObject> grid;
    public int x;
    public int y;
    private GameObject itemImage;

    public Items item;
    public Items tempItem;

    //class constructor
    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        item = Items.None;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + item;
    }

    //changes what object placed in this grid object
    public void SetItem(Items itemName)
    {
        this.item = itemName;
        if (itemImage == null)
        {
            itemImage = GameObject.Instantiate(uiPrefab, new Vector3(0, 0, 0) * grid.GetCellSize(), Quaternion.identity, inventoryTab.transform);
        }

        itemImage.GetComponentInChildren<Image>().sprite = GetItem(itemName).itemSprite;
        itemImage.GetComponentsInChildren<RectTransform>()[1].sizeDelta = grid.GetCellSize() * GetItem(itemName).itemSize;
        itemImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0) * grid.GetCellSize();
        //itemImage.GetComponentInChildren<InteractableObject>().item = item;
        itemImage.SetActive(true);

        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //clear item from the gridobject
    public void ClearItem()
    {
        item = Items.None;

        if (itemImage != null)
        {
            itemImage.SetActive(false);
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
        tempItem = itemName;
    }

    public bool EmptyTemp()
    {
        return tempItem == null;
    }

    public void ClearTemp()
    {
        tempItem = Items.None;
    }

    public Item GetTemp(Items itemName)
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

    public void SetTempAsReal()
    {
        ClearItem();
        if (!EmptyTemp())
        {
            SetItem(tempItem);
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