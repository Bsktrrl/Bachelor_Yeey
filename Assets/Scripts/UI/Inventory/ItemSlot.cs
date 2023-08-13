using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            //if there are any item on this GameObject
            if (transform.childCount > 0)
            {
                //If the item type is the same, add it to the amount

                //If the stack are full, don't do anything

                return transform.GetChild(0).gameObject;
            }

            return null;
        }
    }


    //--------------------


    //If something is dropped on this GameObject
    public void OnDrop(PointerEventData eventData)
    {
        for (int i = 0; i < InventorySystem.instance.inventorySlotList.Count; i++)
        {
            if (InventorySystem.instance.inventorySlotList[i].GetComponent<ItemSlot>() == this)
            {
                InventorySystem.instance.targetInventorySlotList_Index = i;

                break;
            }
        }

        InventoryItem temp = new InventoryItem();

        temp = InventorySystem.instance.inventoryItemList[InventorySystem.instance.activeInventorySlotList_Index];
        InventorySystem.instance.inventoryItemList[InventorySystem.instance.activeInventorySlotList_Index] = InventorySystem.instance.inventoryItemList[InventorySystem.instance.targetInventorySlotList_Index];
        InventorySystem.instance.inventoryItemList[InventorySystem.instance.targetInventorySlotList_Index] = temp;

        DragDrop.itemBeingDragged.transform.SetParent(DragDrop.itemBeingDragged.GetComponent<DragDrop>().startParent);
        DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);

        InventorySystem.instance.UpdateInventoryDisplay();
    }
}