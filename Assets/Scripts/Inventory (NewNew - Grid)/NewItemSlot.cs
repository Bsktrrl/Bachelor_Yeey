using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewItemSlot : MonoBehaviour, IPointerUpHandler
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
                NewGridInventoryManager.instance.RemoveItemFromInventory(inventoryIndex, itemName);
            }

            //If the left Mouse button is pressed - Mark this item to an available Hotbar space
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                print("PlayerInventory - Left");
                //Execute - Select item to Hotbar
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
                    NewGridInventoryManager.instance.MoveItemToInventory(inventoryIndex, gameObject);
                }

                //Move from chest to Player Inventory
                else
                {
                    NewGridInventoryManager.instance.MoveItemToInventory(inventoryIndex, gameObject);
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
