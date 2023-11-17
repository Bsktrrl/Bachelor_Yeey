using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridInventoryManager : MonoBehaviour
{
    public static GridInventoryManager instance { get; private set; } //Singleton

    [Header("Items")]
    public Item_SO item_SO;

    public Vector2 SetInventorySizeTemp = new Vector2(5, 7); //Temp
    public Vector2 SetSmallChestInventorySize = new Vector2(5, 3); //15
    public Vector2 SetMediumChestInventorySize = new Vector2(5, 7); //35
    public Vector2 SetBigChestInventorySize = new Vector2(7, 8); //56
    public float cellSize = 100f;

    [Header("All Inventories")]
    public List<GridInventory> inventories = new List<GridInventory>();

    public List<GameObject> WorldItemTempList = new List<GameObject>(); //Temporary

    [Header("The Inventory Grid")]
    [SerializeField] Image playerInevntory_BackgroundImage;
    public Image chestInevntory_BackgroundImage;
    public List<Grid<GridObject>> gridList = new List<Grid<GridObject>>();

    public Item fillerItem_Prefab;

    public GameObject playerInventory_Parent;
    public GameObject chestInventory_Parent;
    public GameObject uiPrefab;
    [SerializeField] GameObject itemDropPoint;
    public int chestIndexOpen;

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
        gridList.Clear();
        gridList.Add(new Grid<GridObject>((int)SetInventorySizeTemp.x, (int)SetInventorySizeTemp.y, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y)));

        GridObject.inventoryTab = playerInventory_Parent;
        GridObject.uiPrefab = uiPrefab;
    }
    private void Start()
    {
        DataManager.datahasLoaded += LoadData;
        PlayerButtonManager.Tab_isPressedDown += OpenPlayerInventory;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;
        PlayerButtonManager.E_isPressedDown += CloseInventoryScreen;

        fillerItem_Prefab = item_SO.itemList[0];
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            SetInventorySize(0, SetInventorySizeTemp);

            print("New Inventory Size Set");
        }
    }


    //--------------------


    public void LoadData()
    {
        inventories = DataManager.instance.gridInventories_StoreList;
        print("Load_Inventories");

        //Safty Inventory check if starting a new game - Always have at least 1 inventory
        if (inventories.Count <= 0)
        {
            print("AddInventory");
            AddInventory(new Vector2(5, 7));

            SaveData();
        }

        SetInventorySize(0, inventories[0].inventorySize);
    }

    public void SaveData()
    {
        DataManager.instance.gridInventories_StoreList = inventories;
        //print("Save_Inventories");
    }
    public void SaveData(ref GameData gameData)
    {
        DataManager.instance.gridInventories_StoreList = inventories;
        print("Save_Inventories");
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
        if (index <= 0)
        {
            for (int i = playerInventory_Parent.transform.childCount - 1; i >= 0; i--)
            {
                playerInventory_Parent.transform.GetChild(i).gameObject.GetComponent<GridItemSlot>().DestroyObject();
            }
        }
        else
        {
            for (int i = chestInventory_Parent.transform.childCount - 1; i >= 0; i--)
            {
                chestInventory_Parent.transform.GetChild(i).gameObject.GetComponent<GridItemSlot>().DestroyObject();
            }
        }
        
        inventories[index].inventorySize = size;

        Grid<GridObject> tempgrid = new Grid<GridObject>((int)size.x, (int)size.y, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        gridList.Insert(index, tempgrid);
        gridList.RemoveAt(index + 1);

        SaveData();
    }
    public void ResetItemInventoryPlacements(GridInventory inventory)
    {
        //print("playerInventory_Parent.transform.childCount: " + playerInventory_Parent.transform.childCount);

        if (inventory.index <= 0)
        {
            for (int i = 0; i < playerInventory_Parent.transform.childCount; i++)
            {
                InteractableObject interactableObject = playerInventory_Parent.transform.GetChild(i).GetComponent<InteractableObject>();

                //playerInventory_Parent.transform.GetChild(i).GetComponent<InteractableObject>().item.itemName =
                //        playerInventory_Parent.transform.GetChild(i).GetComponent<InteractableObject>().itemName;

                interactableObject.inventoryIndex = 0;
                interactableObject.size = GetItem(interactableObject.itemName).itemSize;
            }
        }
        else
        {
            for (int i = 0; i < chestInventory_Parent.transform.childCount; i++)
            {
                InteractableObject interactableObject = chestInventory_Parent.transform.GetChild(i).GetComponent<InteractableObject>();

                //chestInventory_Parent.transform.GetChild(i).GetComponent<InteractableObject>().item.itemName =
                //        chestInventory_Parent.transform.GetChild(i).GetComponent<InteractableObject>().itemName;

                interactableObject.inventoryIndex = chestIndexOpen;
                interactableObject.size = GetItem(interactableObject.itemName).itemSize;
            }
        }
    }


    //--------------------


    public bool AddItemToInventory(int inventory, GameObject obj, Items itemName, bool remove)
    {
        if (checkInventorySpace(inventories[inventory], obj, remove))
        {
            SoundManager.instance.Playmenu_AddItemToInevntory_Clip();

            inventories[inventory].itemsInInventory.Add(obj.GetComponent<InteractableObject>().item);

            print("200. checkInventorySpace - ItemName: = " + obj.GetComponent<InteractableObject>().itemName);

            //inventories[inventory].itemsInInventory[inventories[inventory].itemsInInventory.Count - 1].isInInventory = inventory;
            //inventories[inventory].itemsInInventory[inventories[inventory].itemsInInventory.Count - 1].itemName = GetItem(itemName).itemName;
            //inventories[inventory].itemsInInventory[inventories[inventory].itemsInInventory.Count - 1].itemSize = GetItem(itemName).itemSize;

            //Safety check for not getting Items.None in any inventory
            for (int i = 0; i < inventories[inventory].itemsInInventory.Count; i++)
            {
                if (inventories[inventory].itemsInInventory[i].itemName == Items.None)
                {
                    RemoveItemFromInventory(inventory, Items.None, false);

                    return false;
                }
            }
            
            //Sort the inventory based on the new item
            if (!SortItems(inventories[inventory]))
            {
                //If the item is to large to place in the inventory, remove the item from the inventory
                Items itemNameTemp = inventories[inventory].itemsInInventory[inventories[inventory].itemsInInventory.Count - 1].itemName;
                
                if (remove)
                {
                    RemoveItemFromInventory(inventory, itemNameTemp, true);
                }
                else
                {
                    RemoveItemFromInventory(inventory, itemNameTemp, false);
                }

                print("2. Inventory if full! | itemNametemp: " + itemNameTemp);

                SaveData();

                return false;
            }
            else
            {
                SaveData();

                return true;
            }
        }
        else
        {
            return false;
        }
    }
    public void RemoveItemFromInventory(int inventory, Items itemName, bool spawnInWorld)
    {
        for (int i = 0; i < inventories[inventory].itemsInInventory.Count; i++)
        {
            if (inventories[inventory].itemsInInventory[i].itemName == itemName)
            {
                //Remove item from the selected inventory
                inventories[inventory].itemsInInventory.RemoveAt(i);

                //Sort inventory
                SortItems(inventories[inventory]);

                ResetItemInventoryPlacements(inventories[inventory]);

                if (spawnInWorld)
                {
                    print("100. spawn in world");

                    SpawnItemInWorld(inventory, itemName);
                }

                SaveData();

                break;
            }
        }
    }
    public void SpawnItemInWorld(int inventory, Items itemName)
    {
        //Drop the item into the World
        print("DropItemIntoTheWorld");
        WorldItemTempList.Add(Instantiate(GetItem(itemName).worldObjectPrefab, itemDropPoint.transform.position, Quaternion.identity) as GameObject);
        WorldItemTempList[WorldItemTempList.Count - 1].GetComponent<Rigidbody>().isKinematic = false;
        WorldItemTempList[WorldItemTempList.Count - 1].GetComponent<Rigidbody>().useGravity = true;
    }


    //--------------------


    bool checkInventorySpace(GridInventory inventory, GameObject obj, bool spawnItemInWorld)
    {
        int itemSize = ((int)obj.GetComponent<InteractableObject>().item.itemSize.x * (int)obj.GetComponent<InteractableObject>().item.itemSize.y);
        int tempGridSize = ((int)inventory.inventorySize.x * (int)inventory.inventorySize.y);
        int tempInventorySpaceUsed = 0;

        for (int i = 0; i < inventory.itemsInInventory.Count; i++)
        {
            tempInventorySpaceUsed += ((int)inventory.itemsInInventory[i].itemSize.x * (int)inventory.itemsInInventory[i].itemSize.y);
        }

        int inventorySpaceLeft = tempGridSize - tempInventorySpaceUsed;

        if (inventorySpaceLeft - itemSize < 0 && spawnItemInWorld)
        {
            SpawnItemInWorld(inventory.index, obj.GetComponent<InteractableObject>().item.itemName);

            print("1. Inventory is full!");
            SoundManager.instance.Playmenu_InventoryIsFull_Clip();

            return false;
        }

        return true;
    }

    public Item GetItem(Items itemName)
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
    public bool SortItems(GridInventory inventory)
    {
        //print("Inventory Size: | X: " + inventory.inventorySize.x + " | Y: " + inventory.inventorySize.y);

        //Sort items by size
        if (inventory.itemsInInventory.Count > 0)
        {
            List<GridInventoryItem> sortedList = inventory.itemsInInventory.OrderByDescending(item => item.itemSize.x * item.itemSize.y).ToList();

            //place items systematically
            foreach (GridInventoryItem item in sortedList)
            {
                bool hasSpot = AvailableSpot(GetItem(item.itemName), inventory);
                if (hasSpot == false)
                {
                    ResetTempValues(inventory);

                    return false;
                }
            }
        }

        foreach (GridObject obj in gridList[inventory.index].gridArray)
        {
            if (inventory.index <= 0)
            {
                obj.SetTempAsReal(playerInventory_Parent);
            }
            else
            {
                obj.SetTempAsReal(chestInventory_Parent);
            }
        }

        return true;
    }

    //check through every spot to find the next available spot
    bool AvailableSpot(Item item, GridInventory inventory)
    {
        for (int y = (int)inventory.inventorySize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < (int)inventory.inventorySize.x; x++)
            {
                //check if the spot is empty
                if (gridList[inventory.index].GetGridObject(x, y) != null)
                {
                    if (gridList[inventory.index].GetGridObject(x, y).EmptyTempItem())
                    {
                        //check if size one
                        if (item.itemSize == Vector2.one)
                        {
                            AssignItemToSpot(item.itemName, x, y, inventory);

                            return true;
                        }
                        else
                        {
                            if (CheckIfFits(item, inventory, new Vector2(x, y)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        //after checking every coordinate and no spots is found
        return false;
    }

    bool CheckIfFits(Item item, GridInventory inventory, Vector2 gridCoordinate)
    {
        List<Vector2> coordsToCheck = new List<Vector2>();

        //get all the coordinates based on the size of the item
        for (int x = 0; x < item.itemSize.x; x++)
        {
            for (int y = 0; y > -item.itemSize.y; y--)
            {
                //if one of the coords is out of bounds, return false
                if ((x + gridCoordinate.x) >= (int)inventory.inventorySize.x || (gridCoordinate.y + y) >= (int)inventory.inventorySize.y)
                {
                    return false;
                }

                coordsToCheck.Add(new Vector2(x + gridCoordinate.x, gridCoordinate.y + y));
            }
        }

        //check all the coordinates
        foreach (Vector2 coord in coordsToCheck)
        {
            if (gridList[inventory.index].GetGridObject((int)coord.x, (int)coord.y) != null)
            {
                if (!gridList[inventory.index].GetGridObject((int)coord.x, (int)coord.y).EmptyTempItem())
                {
                    //if there is something in one of these coordinates, return false
                    return false;
                }
            }
        }

        //return true
        if (AssignItemToSpot(item.itemName, coordsToCheck, inventory))
        {
            return true;
        }
        else
        {
            return false;
        }  
    }

    bool AssignItemToSpot(Items itemName, List<Vector2> itemSizeList, GridInventory inventory)
    {
        for (int i = 0; i < itemSizeList.Count; i++)
        {
            int x = (int)itemSizeList[i].x;
            int y = (int)itemSizeList[i].y;

            if (i != 0)
            {
                if (gridList[inventory.index].GetGridObject(x, y) != null)
                {
                    gridList[inventory.index].GetGridObject(x, y).SetTemp(Items.None);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (gridList[inventory.index].GetGridObject(x, y) != null)
                {
                    gridList[inventory.index].GetGridObject(x, y).SetTemp(itemName);
                }
            }
        }

        return true;
    }
    void AssignItemToSpot(Items itemName, int x, int y, GridInventory inventory)
    {
        gridList[inventory.index].GetGridObject(x, y).SetTemp(itemName);
    }

    void ResetTempValues(GridInventory inventory)
    {
        foreach (GridObject obj in gridList[inventory.index].gridArray)
        {
            if (obj != null)
            {
                obj.ClearTemp();
            }
        }
    }
    #endregion


    //--------------------

    #region Open/Close Invnetory
    public void OpenPlayerInventory()
    {
        if (storageIsOpen)
        {
            CloseInventoryScreen();
        }
        else
        {
            if (MainManager.instance.menuStates == MenuStates.None)
            {
                //Set Player Inventory Size
                SetInventorySize(0, SetInventorySizeTemp);

                //Get PlayerInventory Background
                playerInevntory_BackgroundImage.GetComponent<RectTransform>().sizeDelta = new Vector2(inventories[0].inventorySize.x * cellSize, inventories[0].inventorySize.y * cellSize);

                //Sort Player Inventory
                SortItems(inventories[0]);

                //Reset Player Inventory
                ResetItemInventoryPlacements(inventories[0]);

                storageIsOpen = true;

                Cursor.lockState = CursorLockMode.None;
                MainManager.instance.menuStates = MenuStates.InventoryMenu;

                playerInventory_Parent.SetActive(true);
            }
        }
    }
    void CloseInventoryScreen()
    {
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu || MainManager.instance.menuStates == MenuStates.chestMenu)
        {
            playerInventory_Parent.SetActive(false);
            chestInventory_Parent.SetActive(false);

            storageIsOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
            MainManager.instance.menuStates = MenuStates.None;

            PlayerButtonManager.instance.buttonClickedState = ButtonClickedState.None;
            PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;
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

    public int isInInventory;
}