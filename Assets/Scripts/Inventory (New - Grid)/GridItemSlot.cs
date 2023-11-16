using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridItemSlot : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        //If only player inventory is used
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            //If the left Mouse button is pressed - Remove item from inventory
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                InteractableObject thisItem = gameObject.GetComponent<InteractableObject>();

                CalculateItemToRemove(thisItem, GridInventoryManager.instance.playerInventory_Parent);
            }

            //If the right Mouse button is pressed - Mark this item to an available Hotbar space
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                //Execute - Selection to Hotbar
            }
        }

        //If player is in a chest
        else if (MainManager.instance.menuStates == MenuStates.chestMenu)
        {
            InteractableObject thisItem = gameObject.GetComponent<InteractableObject>();

            //If the left Mouse button is pressed - Remove item from inventory
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ////From Player inventory
                //if (thisItem.inventoryIndex <= 0)
                //{
                //    CalculateItemToRemove(thisItem, GridInventoryManager.instance.playerInventory_Parent);
                //}

                ////From Chest inventory
                //else
                //{
                //    CalculateItemToRemove(thisItem, GridInventoryManager.instance.chestInventory_Parent);
                //}
            }

            //If the right Mouse button is pressed - Move this item between open inventories, if possible
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                //Move from Player Inventory to chest
                if (thisItem.inventoryIndex <= 0)
                {
                    Items tempName = CalculateItemToMove(thisItem, GridInventoryManager.instance.playerInventory_Parent);
                    print("1. tempName = " + tempName);

                    if (tempName != Items.None)
                    {
                        if (GridInventoryManager.instance.AddItemToInventory(GridInventoryManager.instance.chestIndexOpen, gameObject, tempName, false))
                        {
                            //Remove item from old inventory
                            GridInventoryManager.instance.RemoveItemFromInventory(0, tempName, false);
                        }
                    }
                }

                //Move from chest to Player Inventory
                else
                {
                    Items tempName = CalculateItemToMove(thisItem, GridInventoryManager.instance.chestInventory_Parent);
                    print("2. tempName = " + tempName);

                    if (tempName != Items.None)
                    {
                        if (GridInventoryManager.instance.AddItemToInventory(0, gameObject, tempName, false))
                        {
                            //Remove item from old inventory
                            GridInventoryManager.instance.RemoveItemFromInventory(GridInventoryManager.instance.chestIndexOpen, tempName, false);
                        }
                    }
                }
            }
        }
    }

    void CalculateItemToRemove(InteractableObject thisItem, GameObject parent)
    {
        //If itemName == None, find the item that is overlapping with this
        if (thisItem.itemName == Items.None)
        {
            //Calculate if another item is placed on this.gameObject's coordinate
            List<InteractableObject> inventoryItemsTemp = new List<InteractableObject>();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                inventoryItemsTemp.Add(parent.transform.GetChild(i).gameObject.GetComponent<InteractableObject>());
            }

            //Search through all itemSlots in the grid
            for (int i = 0; i < inventoryItemsTemp.Count; i++)
            {
                //If an itemSlot contain an item, search if this.pos == any of the items pos in the grid
                if (inventoryItemsTemp[i].item.itemName != Items.None)
                {
                    Vector2 pos = inventoryItemsTemp[i].item.itemSize;

                    //Search thorugh all pos of the item
                    for (int x = 0; x < pos.x; x++)
                    {
                        for (int y = 0; y < pos.y; y++)
                        {
                            if (thisItem.itemPos == new Vector2(inventoryItemsTemp[i].gameObject.GetComponent<RectTransform>().anchoredPosition.x + (x * GridInventoryManager.instance.cellSize), inventoryItemsTemp[i].gameObject.GetComponent<RectTransform>().anchoredPosition.y - (y * GridInventoryManager.instance.cellSize)))
                            {
                                GridInventoryManager.instance.RemoveItemFromInventory(thisItem.item.isInInventory, inventoryItemsTemp[i].item.itemName, true);

                                return;
                            }
                        }
                    }
                }
            }
        }

        //If itemName != None, remove it as normal
        else
        {
            GridInventoryManager.instance.RemoveItemFromInventory(thisItem.item.isInInventory, thisItem.itemName, true);
        }
    }
    Items CalculateItemToMove(InteractableObject thisItem, GameObject parent)
    {
        //If itemName == None, find the item that is overlapping with this
        if (thisItem.itemName == Items.None)
        {
            //Calculate if another item is placed on this.gameObject's coordinate
            List<InteractableObject> inventoryItemsTemp = new List<InteractableObject>();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                inventoryItemsTemp.Add(parent.transform.GetChild(i).gameObject.GetComponent<InteractableObject>());
            }

            //Search through all itemSlots in the grid
            for (int i = 0; i < inventoryItemsTemp.Count; i++)
            {
                //If an itemSlot contain an item, search if this.pos == any of the items pos in the grid
                if (inventoryItemsTemp[i].item.itemName != Items.None)
                {
                    Vector2 pos = inventoryItemsTemp[i].item.itemSize;

                    //Search thorugh all pos of the item
                    for (int x = 0; x < pos.x; x++)
                    {
                        for (int y = 0; y < pos.y; y++)
                        {
                            if (thisItem.itemPos == new Vector2(inventoryItemsTemp[i].gameObject.GetComponent<RectTransform>().anchoredPosition.x + (x * GridInventoryManager.instance.cellSize), inventoryItemsTemp[i].gameObject.GetComponent<RectTransform>().anchoredPosition.y - (y * GridInventoryManager.instance.cellSize)))
                            {
                                return inventoryItemsTemp[i].gameObject.GetComponent<InteractableObject>().itemName;
                            }
                        }
                    }
                }
            }
        }

        //If itemName != None, remove it as normal
        else
        {
            return thisItem.itemName;
        }

        return thisItem.itemName;
    }


    //--------------------


    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
