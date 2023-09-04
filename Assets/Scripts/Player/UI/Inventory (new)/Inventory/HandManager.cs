using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager instance { get; private set; } //Singleton

    public List<Items> handList = new List<Items>();
    public int selectedSlot;
    public Item selectedSlotItem = new Item();


    //--------------------


    private void Awake()
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
    }
    private void Start()
    {
        PlayerButtonManager.handSelection_Down += HandSelection_Down;
        PlayerButtonManager.handSelection_Up += HandSelection_UP;
        PlayerButtonManager.leftMouse_isPressedDown += UseItem;

        PlayerButtonManager.isPressed_1 += QuickHandSelect_0;
        PlayerButtonManager.isPressed_2 += QuickHandSelect_1;
        PlayerButtonManager.isPressed_3 += QuickHandSelect_2;
        PlayerButtonManager.isPressed_4 += QuickHandSelect_3;
        PlayerButtonManager.isPressed_5 += QuickHandSelect_4;
        PlayerButtonManager.isPressed_6 += QuickHandSelect_5;
        PlayerButtonManager.isPressed_7 += QuickHandSelect_6;
        PlayerButtonManager.isPressed_8 += QuickHandSelect_7;
        PlayerButtonManager.isPressed_9 += QuickHandSelect_8;

        selectedSlot = 0;
    }
    private void Update()
    {
        if (StorageManager.instance.PlayerInventoryItemSlotList.Count > 10)
        {
            for (int i = 0; i < handList.Count; i++)
            {
                handList[i] = StorageManager.instance.PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName;
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (selectedSlot == i)
            {
                StorageManager.instance.PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().selectedInHand.SetActive(true);
            }
            else
            {
                StorageManager.instance.PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().selectedInHand.SetActive(false);
            }
        }
    }


    //--------------------


    void HandSelection_Down()
    {
        selectedSlot--;

        if (selectedSlot < 0)
        {
            selectedSlot = 8;
        }

        UpdateSlotInfo();
    }
    void HandSelection_UP()
    {
        selectedSlot++;

        if (selectedSlot > 8)
        {
            selectedSlot = 0;
        }

        UpdateSlotInfo();
    }

    public void UpdateSlotInfo()
    {
        //Find correct item
        for (int i = 0; i < StorageManager.instance.item_SO.itemList.Count; i++)
        {
            if (StorageManager.instance.item_SO.itemList[i].itemName == StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName)
            {
                UpdateItemInfo(selectedSlotItem, i);

                break;
            }
        }

        EquipmentManager.instance.DisplayEquippedModel(selectedSlotItem);
    }
    void UpdateItemInfo(Item selectedSlotItem, int i)
    {
        selectedSlotItem.itemName = StorageManager.instance.item_SO.itemList[i].itemName;
        selectedSlotItem.categoryName = StorageManager.instance.item_SO.itemList[i].categoryName;
        selectedSlotItem.subCategoryName = StorageManager.instance.item_SO.itemList[i].subCategoryName;

        selectedSlotItem.itemSprite = StorageManager.instance.item_SO.itemList[i].itemSprite;
        selectedSlotItem.equippedPrefab = StorageManager.instance.item_SO.itemList[i].equippedPrefab;

        selectedSlotItem.itemDescription = StorageManager.instance.item_SO.itemList[i].itemDescription;


        selectedSlotItem.isActive = StorageManager.instance.item_SO.itemList[i].isActive;
        selectedSlotItem.itemStackMax = StorageManager.instance.item_SO.itemList[i].itemStackMax;
        selectedSlotItem.isEquipable = StorageManager.instance.item_SO.itemList[i].isEquipable;
        selectedSlotItem.HP = StorageManager.instance.item_SO.itemList[i].HP;

        selectedSlotItem.isCrafteable = StorageManager.instance.item_SO.itemList[i].isCrafteable;
        selectedSlotItem.craftingRequirements = StorageManager.instance.item_SO.itemList[i].craftingRequirements;
    }

    #region QuickSlots
    void QuickHandSelect_0()
    {
        selectedSlot = 0;
        UpdateSlotInfo();
    }
    void QuickHandSelect_1()
    {
        selectedSlot = 1;
        UpdateSlotInfo();
    }
    void QuickHandSelect_2()
    {
        selectedSlot = 2;
        UpdateSlotInfo();
    }
    void QuickHandSelect_3()
    {
        selectedSlot = 3;
        UpdateSlotInfo();
    }
    void QuickHandSelect_4()
    {
        selectedSlot = 4;
        UpdateSlotInfo();
    }
    void QuickHandSelect_5()
    {
        selectedSlot = 5;
        UpdateSlotInfo();
    }
    void QuickHandSelect_6()
    {
        selectedSlot = 6;
        UpdateSlotInfo();
    }
    void QuickHandSelect_7()
    {
        selectedSlot = 7;
        UpdateSlotInfo();
    }
    void QuickHandSelect_8()
    {
        selectedSlot = 8;
        UpdateSlotInfo();
    }
    #endregion

    void UseItem()
    {
        //Spawn Small Chest
        if (selectedSlotItem.itemName == Items.SmallChest)
        {
            StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= 1;

            if (StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
            {
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                selectedSlotItem = StorageManager.instance.item_SO.itemList[0];
            }

            WorldObjectManager.instance.AddChestIntoWorld(5);
        }
        //Spawn Medium Chest
        else if (selectedSlotItem.itemName == Items.MediumChest)
        {
            StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= 1;

            if (StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
            {
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                selectedSlotItem = StorageManager.instance.item_SO.itemList[0];
            }

            WorldObjectManager.instance.AddChestIntoWorld(15);
        }
    }
}
