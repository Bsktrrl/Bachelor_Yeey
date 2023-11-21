using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerUpHandler
{
    public Items itemName = Items.None;
    public int itemID;
    public int inventoryIndex;


    //--------------------


    public void OnPointerUp(PointerEventData eventData)
    {
        //print("You clicked on item: " + itemName + " with index: " + itemID + " and from inventory: " + inventoryIndex);

        //If only player inventory is used
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            //If the right Mouse button is pressed - Remove item from inventory
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                print("PlayerInventory - Right");
                InventoryManager.instance.RemoveItemFromInventory(inventoryIndex, itemName);
            }

            //If the left Mouse button is pressed - Mark this item to an available Hotbar space
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                print("PlayerInventory - Left");
                if (itemName != Items.None)
                {
                    //Check if item is already on the Hotbar
                    for (int i = 0; i < HotbarManager.instance.hotbarList.Count; i++)
                    {
                        if (HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem != Items.None
                            && HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem == itemName)
                        {
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem = Items.None;
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().RemoVeHotbarSlotImage();
                            HotbarManager.instance.SetSelectedItem();

                            return;
                        }
                    }

                    //Add item to the Hotbar if item isn't already on the Hotbar
                    for (int i = 0; i < HotbarManager.instance.hotbarList.Count; i++)
                    {
                        if (HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem == Items.None)
                        {
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem = itemName;
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().SetHotbarSlotImage();
                            HotbarManager.instance.SetSelectedItem();

                            return;
                        }
                    }
                }
            }
        }

        //If player is in a chest
        else if (MainManager.instance.menuStates == MenuStates.chestMenu)
        {
            //If the left Mouse button is pressed - Move this item between the open inventories, if possible
            if (eventData.button == PointerEventData.InputButton.Right && itemName != Items.None)
            {
                print("ChestInventory - Right");
                //Move from Player Inventory to chest
                if (inventoryIndex <= 0)
                {
                    InventoryManager.instance.MoveItemToInventory(inventoryIndex, gameObject);
                }

                //Move from chest to Player Inventory
                else
                {
                    InventoryManager.instance.MoveItemToInventory(inventoryIndex, gameObject);
                }
            }

            //If the left Mouse button is pressed - Mark this item to an available Hotbar space
            else if (eventData.button == PointerEventData.InputButton.Left && inventoryIndex <= 0)
            {
                print("PlayerInventory - Left");
                if (itemName != Items.None)
                {
                    //Check if item is already on the Hotbar
                    for (int i = 0; i < HotbarManager.instance.hotbarList.Count; i++)
                    {
                        if (HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem != Items.None
                            && HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem == itemName)
                        {
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem = Items.None;
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().RemoVeHotbarSlotImage();
                            HotbarManager.instance.SetSelectedItem();

                            return;
                        }
                    }

                    //Add item to the Hotbar if item isn't already on the Hotbar
                    for (int i = 0; i < HotbarManager.instance.hotbarList.Count; i++)
                    {
                        if (HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem == Items.None)
                        {
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().hotbarItem = itemName;
                            HotbarManager.instance.hotbarList[i].GetComponent<HotbarSlot>().SetHotbarSlotImage();
                            HotbarManager.instance.SetSelectedItem();

                            return;
                        }
                    }
                }
            }
        }
    }


    //--------------------


    public void DestroyItemSlot()
    {
        Destroy(gameObject);
    }
}
