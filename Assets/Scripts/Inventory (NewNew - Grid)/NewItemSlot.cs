using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewItemSlot : MonoBehaviour, IPointerUpHandler
{
    public Items itemName = Items.None;
    public int itemIndex;
    public int inventoryIndex;


    //--------------------


    public void OnPointerUp(PointerEventData eventData)
    {
        print("You clicked on item: " + itemName + " with index: " + itemIndex + " and from inventory: " + inventoryIndex);

        //If only player inventory is used
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            //If the right Mouse button is pressed - Remove item from inventory
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                NewGridInventoryManager.instance.RemoveItemFromInventory(inventoryIndex, itemName, true);
            }

            //If the left Mouse button is pressed - Mark this item to an available Hotbar space
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                //Execute - Select item to Hotbar
            }
        }

        //If player is in a chest
        else if (MainManager.instance.menuStates == MenuStates.chestMenu)
        {
            //If the left Mouse button is pressed - Move this item between the open inventories, if possible
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //Move from Player Inventory to chest
                if (inventoryIndex <= 0)
                {
                    
                }

                //Move from chest to Player Inventory
                else
                {
                    
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
