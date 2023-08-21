using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class ItemSlot_N : MonoBehaviour, IDropHandler
{
    [Header("Components")]
    public GameObject draggeableSlotObject;
    public DraggeableSlot draggeableSlotScript;

    public int slotIndexInInventory; //"I am this position in the inventory"
    public int itemInThisSlotFromInventory; //"I am from this inventoryIndex from the SaveList"
    public InventoryItem itemInThisSlot; //"I contain of this InventoryItem"

    public int itemInThisSlot_ItemIndex; //"My item if of this number in the item_SO.List" (updates during sorting)

    public bool onDrop;


    //--------------------


    //If something is dropped on this GameObject
    public void OnDrop(PointerEventData eventData)
    {
        onDrop = true;

        SoundManager.instance.PlayDropItem_Clip();

        //Set TargetSlot
        #region
        bool targetItemFound = false;
        if (!targetItemFound)
        {
            for (int i = 0; i < StorageManager.instance.PlayerInventoryItemSlotList.Count; i++)
            {
                if (StorageManager.instance.PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>() == this)
                {
                    StorageManager.instance.targetSlotList = StorageManager.instance.PlayerInventoryItemSlotList;
                    StorageManager.instance.targetSlotList_Index = i;
                    StorageManager.instance.targetInventoryItem = itemInThisSlot;
                    StorageManager.instance.targetInventoryList_Index = itemInThisSlotFromInventory;

                    targetItemFound = true;

                    break;
                }
            }
        }
        if (!targetItemFound)
        {
            for (int i = 0; i < StorageManager.instance.StorageBoxItemSlotList.Count; i++)
            {
                if (StorageManager.instance.StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>() == this)
                {
                    StorageManager.instance.targetSlotList = StorageManager.instance.StorageBoxItemSlotList;
                    StorageManager.instance.targetSlotList_Index = i;
                    StorageManager.instance.targetInventoryItem = itemInThisSlot;
                    StorageManager.instance.targetInventoryList_Index = itemInThisSlotFromInventory;

                    targetItemFound = true;

                    break;
                }
            }
        }
        #endregion

        //ItemSlot Calculation
        #region
        InventoryItem activeInventoryItem = StorageManager.instance.activeInventoryItem;
        InventoryItem targetInventoryItem = StorageManager.instance.targetInventoryItem;

        bool isEmpty = StorageManager.instance.selectedItemIsEmpty;

        bool isSplitted = StorageManager.instance.itemIsSplitted;

        int amountSelected = StorageManager.instance.itemAmountSelected;
        int amountLeftBehind = StorageManager.instance.itemAmountLeftBehind;
        int maxStack = 0;
        for (int i = 0; i < StorageManager.instance.item_SO.itemList.Count; i++)
        {
            if (activeInventoryItem.itemName == StorageManager.instance.item_SO.itemList[i].itemName)
            {
                maxStack = StorageManager.instance.item_SO.itemList[i].itemStackMax;

                break;
            }
        }


        if (!isEmpty)
        {
            //If the target is the active
            if (activeInventoryItem == targetInventoryItem)
            {
                //Reset itemAmount
                ResetAmount(activeInventoryItem);
                activeInventoryItem.amount = StorageManager.instance.maxItemInSelectedStack;

                print("If the target is the active");
            }

            //If draggingItem has split and target is Empty
            else if (isSplitted && targetInventoryItem.itemName == Items.None)
            {
                targetInventoryItem.itemName = activeInventoryItem.itemName;
                targetInventoryItem.amount = amountSelected;

                activeInventoryItem.amount = amountLeftBehind;

                print("If draggingItem has split and target is Empty");
            }

            //If target is Empty
            else if (targetInventoryItem.itemName == Items.None)
            {
                ResetAmount(activeInventoryItem);
                Swap(activeInventoryItem, targetInventoryItem);

                print("If target is Empty");
            }

            //If draggingItem has split (or not) and both target have the same type
            else if ((isSplitted && activeInventoryItem.itemName == targetInventoryItem.itemName)
                || (activeInventoryItem.itemName == targetInventoryItem.itemName))
            {
                //if target amount = max
                if (targetInventoryItem.amount == maxStack)
                {
                    ResetAmount(activeInventoryItem);
                    Swap(activeInventoryItem, targetInventoryItem);
                }
                else
                {
                    //Get the total amount
                    int amountCounter = amountSelected + targetInventoryItem.amount;

                    //If "amountCounter" is more or equal to a stack
                    if (amountCounter > maxStack)
                    {
                        targetInventoryItem.amount = maxStack;
                        amountCounter -= maxStack;

                        if (amountCounter <= 0)
                        {
                            SetEmpty(activeInventoryItem);
                        }
                        else
                        {
                            activeInventoryItem.amount = amountCounter;
                        }
                    }
                    //Else, reset the activeInventoryItem
                    else
                    {
                        if (amountLeftBehind <= 0)
                        {
                            targetInventoryItem.amount = amountCounter;
                            SetEmpty(activeInventoryItem);
                        }
                        else
                        {
                            targetInventoryItem.amount = amountCounter;
                            activeInventoryItem.amount = amountLeftBehind;
                        }
                    }
                }

                print("If draggingItem has split and both target have the same type");
            }

            //If draggingItem has split and target is a different itemType
            else if (isSplitted && targetInventoryItem.itemName != activeInventoryItem.itemName)
            {
                ResetAmount(activeInventoryItem);
                Swap(activeInventoryItem, targetInventoryItem);
            }

            //Swap as normal
            else
            {
                ResetAmount(activeInventoryItem);
                Swap(activeInventoryItem, targetInventoryItem);

                print("Swap as normal");
            }
        }

        if (activeInventoryItem.amount <= 0)
        {
            SetEmpty(activeInventoryItem);
        }
        if (targetInventoryItem.amount <= 0)
        {
            SetEmpty(targetInventoryItem);
        }
        #endregion

        //Set DraggedObject back to its standard position
        if (DraggeableSlot.itemBeingDragged != null)
        {
            if (DraggeableSlot.itemBeingDragged.GetComponent<DraggeableSlot>() != null)
            {
                DraggeableSlot.itemBeingDragged.transform.SetParent(DraggeableSlot.itemBeingDragged.GetComponent<DraggeableSlot>().startParent);
                DraggeableSlot.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
            }
        }

        onDrop = false;
        StorageManager.instance.itemIsSplitted = false;
    }

    void ResetAmount(InventoryItem activeItem)
    {
        activeItem.amount = StorageManager.instance.maxItemInSelectedStack;
    }
    void Swap(InventoryItem activeItem, InventoryItem targetItem)
    {
        InventoryItem temp = new InventoryItem();

        temp = activeItem;
        activeItem = targetItem;
        targetItem = temp;

        StorageManager.instance.targetSlotList[StorageManager.instance.targetSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot = targetItem;
        StorageManager.instance.activeSlotList[StorageManager.instance.activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot = activeItem;
    }
    void SetEmpty(InventoryItem ItemSlot)
    {
        ItemSlot.itemName = Items.None;
        ItemSlot.amount = 0;
    }
}
