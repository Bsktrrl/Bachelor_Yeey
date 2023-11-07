using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridInventoryManager : MonoBehaviour
{
    #region Singleton
    public static GridInventoryManager instance { get; private set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
    }
    #endregion

    [Header("Items")]
    public Item_SO item_SO;

    [Header("All Inventories")]
    public List<GridInventory> inventories = new List<GridInventory>();

    [Header("The Inventory Grid")]
    private Grid<GridObject> grid;
    [SerializeField] int gridWidth = 5;
    [SerializeField] int gridHeight = 7;
    [SerializeField] float cellSize = 100f;

    public InteractableObject fillerItem_Prefab;


    //--------------------


    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        //create the grid
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }
    private void Start()
    {
        DataManager.datahasLoaded += Load_Inventories;
        DataManager.dataIsSaving += Save_Inventories;
    }


    //--------------------


    void Load_Inventories()
    {
        inventories = DataManager.instance.gridInventories_StoreList;

        //Safty Inventory check if starting a new game - Always have at least 1 inventory
        if (inventories.Count <= 0)
        {
            AddInventory(new Vector2(5, 7));
        }
    }
    void Save_Inventories()
    {
        DataManager.instance.gridInventories_StoreList = inventories;
    }


    //--------------------


    public void AddInventory(Vector2 size)
    {
        //Add empty inventory
        GridInventory inventory = new GridInventory();
        inventories.Add(inventory);

        //Set inventory stats
        inventories[inventories.Count - 1].states = InventoryStatus.Empty;
        inventories[inventories.Count - 1].isOpen = false;

        inventories[inventories.Count - 1].index = inventories.Count - 1;
        inventories[inventories.Count - 1].inventorySize = size;

        Save_Inventories();
        print("Inventory Added");
    }
    public void RemoveInventory(int index)
    {
        inventories.RemoveAt(index);

        Save_Inventories();
        print("Inventory removed");
    }
    public void SetInventorySize(int index, Vector2 size)
    {
        inventories[index].inventorySize = size;

        Save_Inventories();
        print("Inventory size set to: (" + inventories[index].inventorySize.x + ", " + inventories[index].inventorySize.y + ")");
    }


    //--------------------


    public bool AddItemToInventory(int inventory, GridInventoryItem item)
    {
        inventories[inventory].itemsInInventory.Add(item);

        //Sort the inventory based on the new item
        if (!SortItems(inventories[inventory].itemsInInventory))
        {
            //If the item is to large to place in the inventory, remove the item from the inventory
            RemoveItemFromInventory(inventory, item.itemName);

            print("inventory full!");
            return false;
        }

        Save_Inventories();
        print("Saved new item in the inventory: " + inventory);
        
        return true;
    }
    public void RemoveItemFromInventory(int inventory, Items itemName)
    {
        for (int i = 0; i < inventories[inventory].itemsInInventory.Count; i++)
        {
            if (inventories[inventory].itemsInInventory[i].itemName == itemName)
            {
                //Remove item from the selected inventory
                inventories.RemoveAt(i);

                //Sort inventory
                SortItems(inventories[inventory].itemsInInventory);

                //Drop the item into the World
                DropItemIntoTheWorld();

                Save_Inventories();
                print("Removed item from its inventories_list");
            }
        }
    }


    //--------------------


    void AssignItemToSpot(Items itemName, List<Vector2> itemSizeList)
    {
        for (int i = 0; i < itemSizeList.Count; i++)
        {
            int x = (int)itemSizeList[i].x;
            int y = (int)itemSizeList[i].y;

            if (i != 0)
            {
                grid.GetGridObject(x, y).SetTemp(Items.None);
            }
            else
            {
                grid.GetGridObject(x, y).SetTemp(itemName);
            }
        }
    }
    void AssignItemToSpot(Items itemName, int x, int y)
    {
        grid.GetGridObject(x, y).SetTemp(itemName);
    }

    void ResetTempValues()
    {
        print("reset temp");

        foreach (GridObject obj in grid.gridArray)
        {
            obj.ClearTemp();
        }
    }

    bool CheckIfFits(Item item, Vector2 gridCoordinate)
    {
        List<Vector2> coordsToCheck = new List<Vector2>();

        //get all the coordinates based on the size of the item
        for (int x = 0; x < item.itemSize.x; x++)
        {
            for (int y = 0; y > -item.itemSize.y; y--)
            {
                //if one of the coords is out of bounds, return false
                if ((x + gridCoordinate.x) >= gridWidth || (gridCoordinate.y + y) >= gridHeight)
                {
                    return false;
                }

                coordsToCheck.Add(new Vector2(x + gridCoordinate.x, gridCoordinate.y + y));
            }
        }

        //check all the coordinates
        foreach (Vector2 coord in coordsToCheck)
        {
            if (!grid.GetGridObject((int)coord.x, (int)coord.y).EmptyTemp())
            {
                //if there is something in one of these coordinates, return false
                return false;
            }
        }

        //return true
        AssignItemToSpot(item.itemName, coordsToCheck);
        return true;
    }

    //check through every spot to find the next available spot
    bool AvailableSpot(Item item)
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //check if the spot is empty
                if (grid.GetGridObject(x, y).EmptyTemp())
                {
                    //check if size one
                    if (item.itemSize == Vector2.one)
                    {
                        AssignItemToSpot(item.itemName, x, y);
                        return true;
                    }
                    else
                    {
                        if (CheckIfFits(item, new Vector2(x, y)))
                        {
                            return true;
                        }
                    }
                }

            }
        }

        //after checking every coordinate, no spots found
        return false;
    }

    //function returns true if all items can be sorted, and sorts them properly
    //returns false if items cannot be sorted, and deletes all the temporary values
    bool SortItems(List<GridInventoryItem> inventoryItemNameList)
    {
        print("SortItems");

        //sort items by size
        if (inventoryItemNameList.Count > 0)
        {
            var sortedList = inventoryItemNameList.OrderByDescending(d => d.itemSize.x * d.itemSize.y);

            //place items systematically
            foreach (GridInventoryItem item in sortedList)
            {
                bool hasSpot = AvailableSpot(GetItem(item.itemName));
                if (hasSpot == false)
                {
                    Debug.Log("doesnt fit!");
                    ResetTempValues();
                    return false;
                }
            }
        }

        foreach (GridObject obj in grid.gridArray)
        {
            obj.SetTempAsReal();
        }

        return true;
    }

    Item GetItem(Items itemName)
    {
        for (int i = 0; i < item_SO.itemList.Count; i++)
        {
            if (item_SO.itemList[i].itemName == itemName)
            {
                return item_SO.itemList[i];
            }
        }

        return null;
    }


    //--------------------


    void DropItemIntoTheWorld()
    {

    }
}


//--------------------


[Serializable]
public class GridInventory
{
    [Header("Status")]
    public InventoryStatus states;
    public bool isOpen;

    [Header("General")]
    public int index;
    public Vector2 inventorySize;

    [Header("List of items in this inventory")]
    public List<GridInventoryItem> itemsInInventory = new List<GridInventoryItem>();
}

[Serializable]
public class GridInventoryItem
{
    public Items itemName;
    public Vector2 itemSize;
}


//--------------------


public enum InventoryStatus
{
    Empty,
    hasItems,
    Full
}