using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance { get; set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
    }

    private Grid<GridObject> grid;

    int gridWidth = 6;
    int gridHeight = 8;
    float cellSize = 100f;

    //list of all items in the inventory
    public List<InventoryItem> inventoryList = new List<InventoryItem>();
    public Item_SO itemList_SO;

    public GameObject inventoryTab;
    public GameObject uiPrefab;
    public bool inventoryOpen;

    public GameObject cam;

    public float playerReach;

    public GameObject fillerItem_Prefab;


    //--------------------


    // Start is called before the first frame update
    void Awake()
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

        GridObject.inventoryTab = inventoryTab;
        GridObject.uiPrefab = uiPrefab;

        //create the grid
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        SortItems();
    }

    private void Start()
    {
        inventoryList = InventoryManager.instance.inventories[0].itemList;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if (!inventoryOpen)
        //    {
        //        Cursor.lockState = CursorLockMode.None;
        //        inventoryTab.SetActive(true);
        //    }
        //    else
        //    {
        //        Cursor.lockState = CursorLockMode.Locked;
        //        inventoryTab.SetActive(false);
        //    }
        //    inventoryOpen = !inventoryOpen;
        //}

        //Item i = HoverObject();

        //if (i != null)
        //{
        //    //check if the player left clicks
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        //pickup item
        //        PickUpItem(i);
        //    }
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    if (EventSystem.current.IsPointerOverGameObject())
        //    {
        //        PointerEventData hoveredObj = ExtendedStandaloneInputModule.GetPointerEventData();
        //        foreach (GameObject currentObj in hoveredObj.hovered)
        //        {
        //            InteractableObject io = currentObj.GetComponent<Item>();
        //            if (io != null)
        //            {
        //                Debug.Log("remove " + io.item);
        //                RemoveItem(io.item);
        //                break;
        //            }
        //        }
        //    }
        //}
    }


    //--------------------


    #region Interacting with items

    Item GetItem(Item item)
    {
        for (int i = 0; i < itemList_SO.itemList.Count; i++)
        {
            if (itemList_SO.itemList[i].itemName == item.itemName)
            {
                return itemList_SO.itemList[i];
            }
        }

        return null;
    }

    //Check if interacting with Object in the world
    InteractableObject HoverObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, playerReach))
        {
            return hit.collider.gameObject.GetComponent<InteractableObject>();
        }

        return null;
    }

    //called when you pick up something
    public bool PickUpItem(InventoryItem itemPicked)
    {
        inventoryList.Add(itemPicked);

        //sort inventory
        if (SortItems() == false)
        {
            //remove it from the inventory list
            inventoryList.Remove(itemPicked);

            //error
            Debug.Log("inventory full!");

            return false;
        }

        return true;
    }

    //remove object from inventory and spawn it in the world
    void RemoveItem(Item item)
    {
        //inventoryList.Remove(item);

        //SortItems();
        //Instantiate(item.worldObjectPrefab, cam.transform.position, Quaternion.identity);
    }

    #endregion

    #region Functions to sort the inventory

    //assign items to gidobjects
    void AssignItemToSpot(Item item, List<Vector2> coords)
    {
        //for (int i = 0; i < coords.Count; i++)
        //{
        //    int x = (int)coords[i].x;
        //    int y = (int)coords[i].y;
        //    if (i != 0)
        //    {
        //        grid.GetGridObject(x, y).SetTemp(fillerItem);
        //    }
        //    else
        //    {
        //        grid.GetGridObject(x, y).SetTemp(item);
        //    }
        //}
    }

    void AssignItemToSpot(Item item, int x, int y)
    {
        //grid.GetGridObject(x, y).SetTemp(item);
    }

    void ResetTempValues()
    {
        Debug.Log("reset temp");
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
            if (!grid.GetGridObject((int)coord.x, (int)coord.y).EmptyTempItem())
            {
                //if there is something in one of these coordinates, return false
                return false;
            }
        }

        //return true
        AssignItemToSpot(item, coordsToCheck);
        return true;
    }

    //check through every spot to find the next available spot
    bool AvailSpot(Item item)
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //check if the spot is empty
                if (grid.GetGridObject(x, y).EmptyTempItem())
                {
                    //check if size one
                    if (item.itemSize == Vector2.one)
                    {
                        AssignItemToSpot(item, x, y);
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
    //returns false if items cannot be sorted and deletes all the temporary values
    bool SortItems()
    {
        //Debug.Log("SortItems");

        ////sort items by size
        //var sortedList = inventoryList.OrderByDescending(s => s.itemSize.x * s.itemSize.y);

        ////place items systematically
        //foreach (Item item in sortedList)
        //{
        //    bool hasSpot = AvailSpot(item);
        //    if (hasSpot == false)
        //    {
        //        Debug.Log("doesnt fit!");
        //        ResetTempValues();
        //        return false;
        //    }
        //}

        //foreach (GridObject obj in grid.gridArray)
        //{
        //    obj.SetTempAsReal();
        //}

        return true;

    }

    #endregion
}