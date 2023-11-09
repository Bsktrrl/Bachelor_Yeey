using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridInventoryManager : MonoBehaviour, IDataPersistance
{
    public static GridInventoryManager instance { get; private set; } //Singleton

    [Header("Items")]
    public Item_SO item_SO;

    [Header("All Inventories")]
    public List<GridInventory> inventories = new List<GridInventory>();

    public List<GameObject> WorldItemTempList = new List<GameObject>(); //Temporary

    [Header("The Inventory Grid")]
    private Grid<GridObject> grid;
    [SerializeField] int gridWidth = 5;
    [SerializeField] int gridHeight = 7;
    [SerializeField] float cellSize = 100f;

    public Item fillerItem_Prefab;

    public GameObject inventoryTab;
    public GameObject uiPrefab;
    [SerializeField] GameObject itemDropPoint;

    bool storageIsOpen;


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

        GridObject.inventoryTab = inventoryTab;
        GridObject.uiPrefab = uiPrefab;
    }
    private void Start()
    {
        PlayerButtonManager.Tab_isPressedDown += OpenPlayerInventory;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;

        //SortItems(inventories[0].itemsInInventory);

        fillerItem_Prefab = item_SO.itemList[0];
    }


    //--------------------


    public void LoadData(GameData gameData)
    {
        print("Load_Inventories");
        inventories = DataManager.instance.gridInventories_StoreList;

        //Safty Inventory check if starting a new game - Always have at least 1 inventory
        if (inventories.Count <= 0)
        {
            print("AddInventory");
            AddInventory(new Vector2(5, 7));

            SaveData();
        }
    }

    public void SaveData()
    {
        print("Save_Inventories");
        DataManager.instance.gridInventories_StoreList = inventories;
    }
    public void SaveData(ref GameData gameData)
    {
        print("Save_Inventories");
        DataManager.instance.gridInventories_StoreList = inventories;
    }


    //--------------------


    public void AddInventory(Vector2 size)
    {
        //Add empty inventory
        GridInventory inventory = new GridInventory();
        inventories.Add(inventory);

        //Set inventory stats
        inventories[inventories.Count - 1].isOpen = false;

        inventories[inventories.Count - 1].index = inventories.Count - 1;
        inventories[inventories.Count - 1].inventorySize = size;

        SaveData();
    }
    public void RemoveInventory(int index)
    {
        inventories.RemoveAt(index);

        SaveData();
    }
    public void SetInventorySize(int index, Vector2 size)
    {
        inventories[index].inventorySize = size;

        SaveData();
    }


    //--------------------


    public bool AddItemToInventory(int inventory, GameObject obj)
    {
        if (checkInventorySpace(inventories[inventory], obj))
        {
            SoundManager.instance.Playmenu_AddItemToInevntory_Clip();

            inventories[inventory].itemsInInventory.Add(obj.GetComponent<InteractableObject>().item);

            //Sort the inventory based on the new item
            SortItems(inventories[inventory].itemsInInventory);

            //if (!SortItems(inventories[inventory].itemsInInventory))
            //{
            //    //If the item is to large to place in the inventory, remove the item from the inventory
            //    RemoveItemFromInventory(inventory, obj);

            //    print("2. Inventory if full!");
            //    return false;
            //}
            //else
            //{
            //    SaveData();
            //}

            SaveData();

            return true;
        }

        return false;
    }
    public void RemoveItemFromInventory(int inventory, Items itemName)
    {
        for (int i = 0; i < inventories[inventory].itemsInInventory.Count; i++)
        {
            if (inventories[inventory].itemsInInventory[i].itemName == itemName)
            {
                //Remove item from the selected inventory
                inventories[inventory].itemsInInventory.RemoveAt(i);

                //Sort inventory
                SortItems(inventories[inventory].itemsInInventory);

                //Drop the item into the World
                print("DropItemIntoTheWorld");
                WorldItemTempList.Add(Instantiate(GetItem(itemName).worldObjectPrefab, itemDropPoint.transform.position, Quaternion.identity) as GameObject);
                WorldItemTempList[WorldItemTempList.Count - 1].GetComponent<Rigidbody>().isKinematic = false;
                WorldItemTempList[WorldItemTempList.Count - 1].GetComponent<Rigidbody>().useGravity = true;

                SaveData();

                break;
            }
        }
    }


    //--------------------


    bool checkInventorySpace(GridInventory inventory, GameObject obj)
    {
        int itemSize = ((int)obj.GetComponent<InteractableObject>().item.itemSize.x * (int)obj.GetComponent<InteractableObject>().item.itemSize.y);
        int tempGridSize = ((int)inventory.inventorySize.x * (int)inventory.inventorySize.y);
        int tempInventorySpaceUsed = 0;

        for (int i = 0; i < inventory.itemsInInventory.Count; i++)
        {
            tempInventorySpaceUsed += ((int)inventory.itemsInInventory[i].itemSize.x * (int)inventory.itemsInInventory[i].itemSize.y);
        }

        int inventorySpaceLeft = tempGridSize - tempInventorySpaceUsed;

        if (inventorySpaceLeft - itemSize < 0)
        {
            print("1. Inventory is full!");
            SoundManager.instance.Playmenu_InventoryIsFull_Clip();

            return false;
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


    #region Sort Items in Inventory
    //function returns true if all items can be sorted, and sorts them properly
    //returns false if items cannot be sorted, and deletes all the temporary values
    void SortItems(List<GridInventoryItem> inventoryItemNameList)
    {
        //Sort items by size
        if (inventoryItemNameList.Count > 0)
        {
            List<GridInventoryItem> sortedList = inventoryItemNameList.OrderByDescending(item => item.itemSize.x * item.itemSize.y).ToList();

            //for (int i = 0; i < sortedList.Count; i++)
            //{
            //    print("Name " + i + " : "+ sortedList[i].itemName);
            //}

            //place items systematically
            foreach (GridInventoryItem item in sortedList)
            {
                bool hasSpot = AvailableSpot(GetItem(item.itemName));
                if (hasSpot == false)
                {
                    ResetTempValues();

                    //return false;
                }
            }
        }

        foreach (GridObject obj in grid.gridArray)
        {
            obj.SetTempAsReal();
        }

        //return true;
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
        foreach (GridObject obj in grid.gridArray)
        {
            obj.ClearTemp();
        }
    }
    #endregion

    //--------------------

    #region Open/Close Invnetory
    void OpenPlayerInventory()
    {
        if (storageIsOpen)
        {
            CloseInventoryScreen();
        }
        else
        {
            if (MainManager.instance.menuStates == MenuStates.None)
            {
                //Sort Player Inventory
                SortItems(inventories[0].itemsInInventory);

                storageIsOpen = true;

                Cursor.lockState = CursorLockMode.None;
                MainManager.instance.menuStates = MenuStates.InventoryMenu;

                inventoryTab.SetActive(true);
            }
        }

    }
    void CloseInventoryScreen()
    {
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            inventoryTab.SetActive(false);

            storageIsOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
            MainManager.instance.menuStates = MenuStates.None;
        }
    }
    #endregion
}


//--------------------


[Serializable]
public class GridInventory
{
    [Header("Status")]
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