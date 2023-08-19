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

    public bool onDrop;

    //--------------------


    //If something is dropped on this GameObject
    public void OnDrop(PointerEventData eventData)
    {
        print("OnDrop");

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

        if (!isEmpty)
        {
            //If the target is the active
            if (activeInventoryItem == targetInventoryItem)
            {
                //Don't do anything

                print("If the target is the active");
            }

            //Swap as normal
            else
            {
                Swap(activeInventoryItem, targetInventoryItem);

                print("Swap as normal");
            }
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
    void SetEmpty(InventoryItem item)
    {
        item.itemName = Items.None;
        item.amount = 0;
    }
}
