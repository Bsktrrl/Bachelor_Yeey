using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("item Stats")]
    public bool playerInRange;

    [Header("Object Type")]
    public ObjectType objectType;

    [Header("If PickUp")]
    public Items itemName;
    [HideInInspector] public Vector2 itemPos;

    [Header("If Inventory")]
    public int inventoryIndex;
    public Vector2 size;

    public Grid<GridObject> grid;

    [HideInInspector] public GridInventoryItem item = new GridInventoryItem();

    SphereCollider accessCollider = new SphereCollider();


    //--------------------


    private void Start()
    {
        PlayerButtonManager.E_isPressedDown += ObjectInteraction;

        //Add SphereCollider for the item
        accessCollider = gameObject.AddComponent<SphereCollider>();
        Vector3 scale = gameObject.transform.lossyScale;
        accessCollider.radius = WorldObjectManager.instance.objectColliderRadius / scale.x / 2; //Chenge to only "objectColliderRadius"
        accessCollider.isTrigger = true;

        //Get correct gridInventoryItem
        for (int i = 0; i < GridInventoryManager.instance.item_SO.itemList.Count; i++)
        {
            if (GridInventoryManager.instance.item_SO.itemList[i].itemName == itemName)
            {
                item.itemName = GridInventoryManager.instance.item_SO.itemList[i].itemName;
                item.itemSize = GridInventoryManager.instance.item_SO.itemList[i].itemSize;

                break;
            }
        }

        //If inventory, get grid and size
        if (objectType == ObjectType.Inventory)
        {
            //Spawn in-game
            if (itemName == Items.SmallChest)
            {
                GridInventoryManager.instance.gridList.Add(new Grid<GridObject>((int)GridInventoryManager.instance.SetSmallChestInventorySize.x, (int)GridInventoryManager.instance.SetSmallChestInventorySize.y, GridInventoryManager.instance.cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y)));
                size = GridInventoryManager.instance.SetSmallChestInventorySize;
            }
            else if (itemName == Items.MediumChest)
            {
                GridInventoryManager.instance.gridList.Add(new Grid<GridObject>((int)GridInventoryManager.instance.SetMediumChestInventorySize.x, (int)GridInventoryManager.instance.SetMediumChestInventorySize.y, GridInventoryManager.instance.cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y)));
                size = GridInventoryManager.instance.SetMediumChestInventorySize;
            }

            //Spawn from Editor
            else
            {
                GridInventoryManager.instance.gridList.Add(new Grid<GridObject>((int)size.x, (int)size.y, GridInventoryManager.instance.cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y)));
            }

            if (inventoryIndex <= 0)
            {
                inventoryIndex = GridInventoryManager.instance.gridList.Count - 1;
            }
        }
    }


    //--------------------


    void ObjectInteraction()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject
            && MainManager.instance.menuStates == MenuStates.None)
        {
            //If Object is a PickUp
            if (objectType == ObjectType.Pickups)
            {
                //Check If item can be added
                GridInventoryManager.instance.AddItemToInventory(0, gameObject, itemName, true);

                //Unsubscribe from Event
                PlayerButtonManager.E_isPressedDown -= ObjectInteraction;

                //Destroy gameObject (If full inventory, drop the item from "RemoveItemFromInventory" in "GridInventorymanager")
                Destroy(gameObject);
            }

            //If Object is an Inventory
            else if (objectType == ObjectType.Inventory)
            {
                GridInventoryManager.instance.chestIndexOpen = inventoryIndex;

                //Set ChestInventory Size
                GridInventoryManager.instance.SetInventorySize(inventoryIndex, size);

                //Get ChestInventory Background
                GridInventoryManager.instance.chestInevntory_BackgroundImage.GetComponent<RectTransform>().sizeDelta = new Vector2(GridInventoryManager.instance.inventories[inventoryIndex].inventorySize.x * GridInventoryManager.instance.cellSize, GridInventoryManager.instance.inventories[inventoryIndex].inventorySize.y * GridInventoryManager.instance.cellSize);

                //Sort ChestInventory
                GridInventoryManager.instance.SortItems(GridInventoryManager.instance.inventories[inventoryIndex]);

                //Reset ChestInventory
                GridInventoryManager.instance.ResetItemInventoryPlacements(GridInventoryManager.instance.inventories[inventoryIndex]);

                //Open Inventory panels
                GridInventoryManager.instance.chestInventory_Parent.SetActive(true);
                GridInventoryManager.instance.OpenPlayerInventory();

                MainManager.instance.menuStates = MenuStates.chestMenu;
            }

            //If Object is an ItemSlot
            else if (objectType == ObjectType.ItemSlot)
            {

            }

            //If Object is another Interacteable
            else if (objectType == ObjectType.Interacteable)
            {
                print("Object is an interactable");
            }

            //If Object is something else
            else
            {
                print("Object is something else");
            }
        }
    }


    //--------------------


    private void OnTriggerEnter(Collider collision)
    {
        //If a player is entering the area
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        //If a player is exiting the area
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

public enum ObjectType
{
    None,

    Pickups,
    Inventory,
    ItemSlot,

    Interacteable
}