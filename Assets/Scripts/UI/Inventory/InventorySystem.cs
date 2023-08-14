using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    //Singleton
    public static InventorySystem instance { get; set; } //Singleton

    [Header("References")]
    public Item_SO SO_Item;
    [SerializeField] Sprite emptySprite;

    [Header("GameObjects")]
    public GameObject inventoryScreenUI;
    public GameObject inventoryDraggingParent;

    [Header("itemDisplay")]
    public Image selecteditemImage;
    public TextMeshProUGUI selecteditemName;
    public TextMeshProUGUI selecteditemDescription;

    [Header("InventorySlots")]
    [SerializeField] GameObject InventorySlot_Parent;
    [SerializeField] GameObject InventorySlot_Prefab;

    [Header("Lists")]
    public List<GameObject> inventorySlotList = new List<GameObject>();
    public List<InventoryItem> inventoryItemList = new List<InventoryItem>();
    public int activeInventorySlotList_Index;
    public int targetInventorySlotList_Index;

    [HideInInspector] public bool isOpen;
    public bool itemIsDragging;

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
    public void UpdateInventoryDisplay()
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemName == Items.None)
            {
                //Set empty ItemSlot if inventoryItemList[i] is empty
                inventorySlotList[i].GetComponentInChildren<DragDrop>().itemImage.sprite = emptySprite;
                inventorySlotList[i].GetComponentInChildren<DragDrop>().amountText.text = "";
            }
            else
            {
                //Find correct item from _SO
                Item itemTemp = FindSO_Item(inventoryItemList[i]);

                //Insert data from this _SO item
                inventorySlotList[i].GetComponentInChildren<DragDrop>().itemImage.sprite = itemTemp.itemSprite;

                for (int j = 0; j < SO_Item.itemList.Count; j++)
                {
                    if (inventoryItemList[i].itemName == SO_Item.itemList[j].itemName)
                    {
                        if (SO_Item.itemList[j].itemStackMax <= 1)
                        {
                            inventorySlotList[i].GetComponentInChildren<DragDrop>().amountText.text = "";
                        }
                        else
                        {
                            inventorySlotList[i].GetComponentInChildren<DragDrop>().amountText.text = inventoryItemList[i].amount.ToString();
                        }

                        break;
                    }
                }
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
    public void RemoveSlotFromInventory(Items itemName)
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

    public void SortInventoryItemsBy_SOPosition()
    {
        List<InventoryItem> inventoryItemListChecker = new List<InventoryItem>();

        //Fill inventoryItemList with items, pushing all items as far to the left as possible
        for (int i = 1; i < SO_Item.itemList.Count; i++)
        {
            int itemAmountCounter = 0;

            //Get Item Amount
            for (int j = 0; j < inventoryItemList.Count;)
            {
                if (inventoryItemList[j].itemName == SO_Item.itemList[i].itemName)
                {
                    itemAmountCounter += inventoryItemList[j].amount;

                    inventoryItemList.RemoveAt(j);
                }
                else
                {
                    j++;
                }
            }

            //Set item in correct order
            while (itemAmountCounter > 0)
            {
                InventoryItem itemTemp = new InventoryItem();

                inventoryItemListChecker.Add(itemTemp);
                inventoryItemListChecker[inventoryItemListChecker.Count - 1].itemName = SO_Item.itemList[i].itemName;

                if (itemAmountCounter >= SO_Item.itemList[i].itemStackMax)
                {
                    inventoryItemListChecker[inventoryItemListChecker.Count - 1].amount = SO_Item.itemList[i].itemStackMax;
                    itemAmountCounter -= SO_Item.itemList[i].itemStackMax;
                }
                else
                {
                    inventoryItemListChecker[inventoryItemListChecker.Count - 1].amount = itemAmountCounter;
                    itemAmountCounter -= itemAmountCounter;
                }
            }
        }

        //Fill the rest with "None"-Items
        for (int i = inventoryItemListChecker.Count; i < inventorySize; i++)
        {
            InventoryItem itemNonetemp = new InventoryItem();

            inventoryItemListChecker.Add(itemNonetemp);
            inventoryItemListChecker[inventoryItemListChecker.Count - 1].itemName = Items.None;
            inventoryItemListChecker[inventoryItemListChecker.Count - 1].amount = 0;
        }

        inventoryItemList = inventoryItemListChecker;

        UpdateInventoryDisplay();
    }
    public void SortInventoryItemsByInventoryPosition()
    {
        List<Items> itemNameList = new List<Items>();
        Items itemNameTemp = new Items();

        //Make a list of unique itemNames in the inventory
        #region
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            //Check if itemNameList is empty
            if (itemNameList.Count <= 0)
            {
                itemNameList.Add(itemNameTemp);
            }
            
            for (int j = 0; j < itemNameList.Count; j++)
            {
                if (inventoryItemList[i].itemName != itemNameList[j] && inventoryItemList[i].itemName != Items.None)
                {
                    itemNameList.Add(itemNameTemp);
                    itemNameList[itemNameList.Count - 1] = inventoryItemList[i].itemName;
                }
            }
        }
        #endregion

        //Sort inventory based on itemNameList
        #region
        List<InventoryItem> inventoryItemListChecker = new List<InventoryItem>();

        //Fill inventoryItemList with items, pushing all items as far to the left as possible
        for (int i = 0; i < itemNameList.Count; i++)
        {
            int itemAmountCounter = 0;

            //Get Item Amount
            for (int j = 0; j < inventoryItemList.Count;)
            {
                if (inventoryItemList[j].itemName == itemNameList[i])
                {
                    itemAmountCounter += inventoryItemList[j].amount;

                    inventoryItemList.RemoveAt(j);
                }
                else
                {
                    j++;
                }
            }

            //Set item in correct order
            while (itemAmountCounter > 0)
            {
                InventoryItem itemTemp = new InventoryItem();

                inventoryItemListChecker.Add(itemTemp);
                inventoryItemListChecker[inventoryItemListChecker.Count - 1].itemName = itemNameList[i];

                for (int j = 0; j < SO_Item.itemList.Count; j++)
                {
                    //Find correct position in the SO_Item.itemList based on the current itemNameList
                    if (itemNameList[i] == SO_Item.itemList[j].itemName)
                    {
                        if (itemAmountCounter >= SO_Item.itemList[j].itemStackMax)
                        {
                            inventoryItemListChecker[inventoryItemListChecker.Count - 1].amount = SO_Item.itemList[j].itemStackMax;
                            itemAmountCounter -= SO_Item.itemList[j].itemStackMax;
                        }
                        else
                        {
                            inventoryItemListChecker[inventoryItemListChecker.Count - 1].amount = itemAmountCounter;
                            itemAmountCounter -= itemAmountCounter;
                        }
                    }
                }
            }
        }

        //Fill the rest with "None"-Items
        for (int i = inventoryItemListChecker.Count; i < inventorySize; i++)
        {
            InventoryItem itemNonetemp = new InventoryItem();

            inventoryItemListChecker.Add(itemNonetemp);
            inventoryItemListChecker[inventoryItemListChecker.Count - 1].itemName = Items.None;
            inventoryItemListChecker[inventoryItemListChecker.Count - 1].amount = 0;
        }

        inventoryItemList = inventoryItemListChecker;
        #endregion

        UpdateInventoryDisplay();
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


    void OpenInventoryScreen()
    {
        if (!isOpen)
        {
            MainManager.instance.menuStates = MenuStates.InventoryMenu;
            UpdateInventoryDisplay();

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

            //Turn off all SelectedFrames
            for (int i = 0; i < inventorySlotList.Count; i++)
            {
                inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>().selectedFrameImage.gameObject.SetActive(false);
            }

            MainManager.instance.menuStates = MenuStates.None;
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

            //Turn off all SelectedFrames
            for (int i = 0; i < inventorySlotList.Count; i++)
            {
                inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>().selectedFrameImage.gameObject.SetActive(false);
            }

            MainManager.instance.menuStates = MenuStates.None;
        }
    }
}

[Serializable]
public class InventoryItem
{
    public Items itemName;
    public int amount;
}

