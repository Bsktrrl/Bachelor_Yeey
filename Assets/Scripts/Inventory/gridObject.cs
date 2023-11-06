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

    public Item item;
    public Item tempItem;

    //class constructor
    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        item = null;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + item.itemName;
    }

    //changes what object placed in this grid object
    public void SetItem(Item item)
    {
        this.item = item;
        if (itemImage == null)
        {
            itemImage = GameObject.Instantiate(uiPrefab, new Vector3(0, 0, 0) * grid.GetCellSize(), Quaternion.identity, inventoryTab.transform);
        }
        itemImage.GetComponentInChildren<Image>().sprite = item.itemSprite;
        itemImage.GetComponentsInChildren<RectTransform>()[1].sizeDelta = grid.GetCellSize() * item.itemSize;
        itemImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0) * grid.GetCellSize();
        //itemImage.GetComponentInChildren<InteractableObject>().item = item;
        itemImage.SetActive(true);

        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //clear item from the gridobject
    public void ClearItem()
    {
        item = null;
        if (itemImage != null)
        {
            itemImage.SetActive(false);
        }
        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //returns the current scriptable object
    public Item GetItem()
    {
        return item;
    }

    //checks if there is no itemscriptableobject in the gridobject
    public bool EmptyItem()
    {
        return item == null;
    }

    public void SetTemp(Item item)
    {
        tempItem = item;
    }

    public bool EmptyTemp()
    {
        return tempItem == null;
    }

    public void ClearTemp()
    {
        tempItem = null;
    }

    public Item GetTemp()
    {
        return tempItem;
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

}