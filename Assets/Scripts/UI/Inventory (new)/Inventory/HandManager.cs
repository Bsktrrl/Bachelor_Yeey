using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager instance { get; private set; } //Singleton

    public List<Items> handList = new List<Items>();
    public int selectedSlot;
    public Items selectedSlotItem;


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
            print("Hand is running");
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

    void HandSelection_Down()
    {
        selectedSlot++;

        if (selectedSlot > 8)
        {
            selectedSlot = 0;
        }

        selectedSlotItem = StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName;
    }
    void HandSelection_UP()
    {
        selectedSlot--;

        if (selectedSlot < 0)
        {
            selectedSlot = 8;
        }

        selectedSlotItem = StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName;
    }

    void UseItem()
    {
        //Spawn Small Chest
        if (selectedSlotItem == Items.SmallChest)
        {
            StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= 1;

            if (StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
            {
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                selectedSlotItem = Items.None;
            }

            WorldObjectManager.instance.AddChestIntoWorld(5);
        }
        //Spawn Medium Chest
        else if (selectedSlotItem == Items.MediumChest)
        {
            StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= 1;

            if (StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
            {
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                StorageManager.instance.PlayerInventoryItemSlotList[selectedSlot].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                selectedSlotItem = Items.None;
            }

            WorldObjectManager.instance.AddChestIntoWorld(15);
        }
    }
}
