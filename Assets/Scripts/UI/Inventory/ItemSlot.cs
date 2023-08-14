using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

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

        #region itemSlot Calculation
        List<InventoryItem> itemList = InventorySystem.instance.inventoryItemList;
        int active = InventorySystem.instance.activeInventorySlotList_Index;
        int target = InventorySystem.instance.targetInventorySlotList_Index;

        InventoryItem temp = new InventoryItem();

        bool isSplitted = InventorySystem.instance.itemIsSplitted;

        int amountSelected = InventorySystem.instance.itemAmountSelected;
        int amountLeftBehind = InventorySystem.instance.itemAmountLeftBehind;

        //If target is active
        if (itemList[target] == itemList[active])
        {
            //Don't do anything

            print("If target is active");
        }

        //If draggingItem has split and target is Empty
        else if (isSplitted && itemList[target].itemName == Items.None)
        {
            itemList[target].itemName = itemList[active].itemName;
            itemList[target].amount = amountSelected;

            if (amountLeftBehind <= 0)
            {
                SetitemEmpty(itemList, active);
            }
            else
            {
                itemList[active].amount = amountLeftBehind;
            }
            
            print("If draggingItem has split and target is Empty");
        }

        //If target is Empty
        else if (itemList[target].itemName == Items.None)
        {
            NormalSwap(itemList, active, target);

            print("If target is Empty");
        }

        //If both targets have the same type
        else if (itemList[active].itemName == itemList[target].itemName)
        {
            //Get the total amount
            int amountCounter = itemList[active].amount + itemList[target].amount;

            //Get StackMax of given Item
            Item type = new Item();
            for (int i = 0; i < InventorySystem.instance.SO_Item.itemList.Count; i++)
            {
                if (itemList[active].itemName == InventorySystem.instance.SO_Item.itemList[i].itemName)
                {
                    type = InventorySystem.instance.SO_Item.itemList[i];

                    break;
                }
            }

            //Fill target as full as possible
            if (amountCounter >= type.itemStackMax)
            {
                itemList[target].amount = type.itemStackMax;
                amountCounter -= type.itemStackMax;
                itemList[active].amount = amountCounter;
            }
            else
            {
                itemList[target].amount = amountCounter;
                amountCounter -= amountCounter;

                //Nullify itemList[active]
                SetitemEmpty(itemList, active);
            }

            print("If both targets have the same type");
        }
        

        //If draggingItem has split and target is a different itemType
        else if (isSplitted && itemList[target].itemName != itemList[active].itemName)
        {
            //Swap as normal
            NormalSwap(itemList, active, target);

            print("If draggingItem has split and target is a different itemType");
        }

        //Swap as normal
        else
        {
            NormalSwap(itemList, active, target);

            print("Swap as normal");
        }
        #endregion

        DragDrop.itemBeingDragged.transform.SetParent(DragDrop.itemBeingDragged.GetComponent<DragDrop>().startParent);
        DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);

        InventorySystem.instance.UpdateInventoryDisplay();
    }

    void NormalSwap(List<InventoryItem> itemList, int a, int b)
    {
        InventoryItem temp = new InventoryItem();

        temp = itemList[a];
        itemList[a] = itemList[b];
        itemList[b] = temp;
    }
    void SetitemEmpty(List<InventoryItem> itemList, int a)
    {
        itemList[a].itemName = Items.None;
        itemList[a].amount = 0;
    }
}