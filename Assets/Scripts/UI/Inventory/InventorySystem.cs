using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance { get; set; } //Singleton

    [Header("References")]
    [SerializeField] Item_SO SO_Item;
    [SerializeField] Sprite emptySprite;

    [Header("GameObjects")]
    public GameObject inventoryScreenUI;
    //public GameObject inventoryPanelUI;
    public GameObject inventoryDraggingParent;


    [Header("InventorySlots")]
    [SerializeField] GameObject InventorySlot_Parent;
    [SerializeField] GameObject InventorySlot_Prefab;

    [Header("Lists")]
    public List<GameObject> inventorySlotList = new List<GameObject>();
    public List<InventoryItem> inventoryItemList = new List<InventoryItem>();

    //public List<string> itemList = new List<string>();

    GameObject itemToAdd;
    GameObject whatSlotToEquip;

    [HideInInspector] public bool isOpen;

    [Header("Inventory Size")]
    [SerializeField] int inventorySize = 15;


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


    void Start()
    {
        PlayerButtonManager.E_isPressedDown += OpenInventoryScreen;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;

        isOpen = false;

        inventoryScreenUI.SetActive(false);

        SetupSlotsInInventory();
        UpdateInventoryDisplay();
    }


    //--------------------


    void SetupSlotsInInventory()
    {
        //Instantiate Slots based on the inventorySize
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlotList.Add(Instantiate(InventorySlot_Prefab) as GameObject);
            inventorySlotList[inventorySlotList.Count - 1].transform.parent = InventorySlot_Parent.transform;
        }

        //Instantiate inventoryItemList based on the inventorySize
        for (int i = 0; i < inventorySize; i++)
        {
            InventoryItem item = new InventoryItem();

            inventoryItemList.Add(item);
        }
    }
    void UpdateInventoryDisplay()
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemName == Items.None)
            {
                //Set empty ItemSlot if inventoryItemList[i] is empty
                inventorySlotList[i].GetComponentInChildren<DragDrop>().gameObject.GetComponentInChildren<Image>().sprite = emptySprite;
                inventorySlotList[i].GetComponentInChildren<DragDrop>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            else
            {
                //Find correct item from _SO
                Item itemTemp = FindSO_Item(inventoryItemList[i]);

                //Insert data from this _SO item
                inventorySlotList[i].GetComponentInChildren<DragDrop>().gameObject.GetComponentInChildren<Image>().sprite = itemTemp.itemSprite;
                inventorySlotList[i].GetComponentInChildren<DragDrop>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text = inventoryItemList[i].amount.ToString();
            }
        }
    }
    Item FindSO_Item(InventoryItem item)
    {
        //Loop through SO_ItemList to find the correct Item
        for (int i = 0; i < SO_Item.itemList.Count; i++)
        {
            if (item.itemName == SO_Item.itemList[i].itemName)
            {
                return SO_Item.itemList[i];
            }
        }

        return null;
    }
    Item FindSO_Item(Items item)
    {
        for (int i = 0; i < SO_Item.itemList.Count; i++)
        {
            if (item == SO_Item.itemList[i].itemName)
            {
                return SO_Item.itemList[i];
            }
        }

        return null;
    }



    //--------------------


    public void AddSlotToInventory()
    {
        //Add ItemSlot


        //Resize Frame


    }
    public void RemoveSlotFromInventory()
    {

    }

    #region Add Item To Inventory
    public bool AddItemToInventory(Items itemName, int amount)
    {
        Item SO_item = FindSO_Item(itemName);
        int amountTemp = amount;

        if (SO_item.itemStackMax <= 0)
        {
            print("itemStackMax <= 0");

            return false;
        }

        while (amountTemp > 0)
        {
            int index = FindItemSlotWithSpace(SO_item, itemName);
            
            if (index > -1)
            {
                int counter = 0;

                counter = SO_item.itemStackMax - inventoryItemList[index].amount;

                if (amountTemp <= counter)
                {
                    inventoryItemList[index].amount += amountTemp;
                    amountTemp -= amountTemp;
                }
                else
                {
                    inventoryItemList[index].amount += counter;
                    amountTemp -= counter;
                }
            }
            else
            {
                //Find an empty ItemSlot
                int emptySlot = FindFirstEmptyItemSlot();

                if (emptySlot > -1)
                {
                    inventoryItemList[emptySlot].itemName = itemName;
                }
                else
                {
                    UpdateInventoryDisplay();

                    return false;
                }
            }
        }

        UpdateInventoryDisplay();

        return true;
    }
    int FindItemSlotWithSpace(Item SO_item, Items itemName)
    {
        //If ItemSlot has the selected item and has room for it
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemName == itemName && inventoryItemList[i].amount < SO_item.itemStackMax)
            {
                return i;
            }
        }

        return -1;
    }
    int FindFirstEmptyItemSlot()
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemName == Items.None)
            {
                return i;
            }
        }

        return -1;
    }
    #endregion

    public void RemoveItemFromInventory()
    {

    }

    public void ItemStackOverflow()
    {

    }
    public bool CheckIfInventorySlotIsFull()
    {
        return false;
    }
    public bool CheckIfInventoryIsFull()
    {
        return false;
    }

    public void SortInventoryItems()
    {

    }

    public void UpdateInventoryItemListOrder()
    {
        //Update the inventoryItemList to match the items and amount in each slot
        List<InventoryItem> inventoryItemListTemp = new List<InventoryItem>();
        inventoryItemListTemp = inventoryItemList;
        inventoryItemList.Clear();

        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            InventoryItem item = new InventoryItem();

            inventoryItemList.Add(item);
            inventoryItemList[inventoryItemList.Count - 1].itemName = FindItemFromInventorySprite(inventorySlotList[i]);
            inventoryItemList[inventoryItemList.Count - 1].amount = int.Parse(inventorySlotList[i].GetComponentInChildren<DragDrop>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text);
        }
    }
    Items FindItemFromInventorySprite(GameObject inventorySlot)
    {
        for (int j = 0; j < SO_Item.itemList.Count; j++)
        {
            if (inventorySlot.GetComponentInChildren<DragDrop>().gameObject.GetComponentInChildren<Image>().sprite == SO_Item.itemList[j].itemSprite)
            {
                return SO_Item.itemList[j].itemName;
            }
        }

        return Items.None;
    }


    //--------------------


    public void AddtoInventory(Items itemName)
    {
        //Insert item into the next available slot
        whatSlotToEquip = FindNextEmptySlot();

        //Find which item to add
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName.ToString()), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        //itemList.Add(itemName.ToString());
    }
    public void RemoveItemFromInventory(List<Requirement> item)
    {
        for (int i = 0; i < item.Count; i++)
        {
            int counter = item[i].itemAmount;

            for (int j = inventorySlotList.Count - 1; j >= 0; j--)
            {
                if (inventorySlotList[j].transform.GetChild(0).name == item[i].itemName.ToString() + "(Clone)" && counter > 0)
                {
                    Destroy(inventorySlotList[j].transform.GetChild(0).gameObject);

                    counter--;
                }
            }
        }
    }
    public void RecalculateList()
    {

    }

    public bool CheckIfFull()
    {
        int counter = 0;

        foreach (GameObject slot in inventorySlotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter++;
            }
        }

        if (counter >= inventorySize)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in inventorySlotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return new GameObject();
    }


    //--------------------


    void OpenInventoryScreen()
    {
        if (!isOpen)
        {
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            isOpen = true;

        }
        else if (isOpen)
        {
            inventoryScreenUI.SetActive(false);

            if (!CraftingSystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            isOpen = false;
        }
    }
    void CloseInventoryScreen()
    {
        if (isOpen)
        {
            inventoryScreenUI.SetActive(false);
            if (!CraftingSystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }
    }
}

[Serializable]
public class InventoryItem
{
    public Items itemName;
    public int amount;
}

