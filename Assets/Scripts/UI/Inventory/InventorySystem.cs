using Newtonsoft.Json.Linq;
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
    public bool itemIsClicked;
    public bool itemIsSplitted;

    public int itemAmountSelected;
    public int maxItemInSelectedStack;
    public int itemAmountLeftBehind;

    public bool selectedItemIsEmpty;

    public bool setupInventory;

    [Header("Inventory Size")]
    [SerializeField] int inventorySize = 15;

    [Header("dragDropTemp")]
    [SerializeField] GameObject dragDropTemp_Prefab;
    public List<GameObject> dragDropTempList = new List<GameObject>();


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

        PlayerButtonManager.inventory_RightMouse_isPressedDown += ItemStack_PickOne;
        PlayerButtonManager.inventory_ScrollMouse_isPressedDown += ItemStack_PickHalf;
        PlayerButtonManager.inventory_ScrollMouse_isRolledUP += IncreaseItemAmountHolding;
        PlayerButtonManager.inventory_ScrollMouse_isRolledDown += DecreaseItemAmountHolding;
        PlayerButtonManager.inventory_Shift_and_RightMouse_isPressedDown += ItemStack_PickAll;

        isOpen = false;

        inventoryScreenUI.SetActive(false);

        SetupSlotsInInventory();
        UpdateInventoryDisplay();
    }
    private void Update()
    {
        if (!itemIsDragging && !itemIsClicked && MainManager.instance.menuStates == MenuStates.InventoryMenu && setupInventory)
        {
            UpdateInventoryDisplay();
        }
    }


    //--------------------


    void SetupSlotsInInventory()
    {
        DeleteSlotsInInventory();

        inventorySlotList.Clear();

        //Set Display as if there wasn't any active itemslot
        selecteditemImage.sprite = SO_Item.itemList[0].itemSprite;
        selecteditemName.text = "";
        selecteditemDescription.text = "";

        //Instantiate Slots based on the inventorySize
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlotList.Add(Instantiate(InventorySlot_Prefab) as GameObject);
            inventorySlotList[inventorySlotList.Count - 1].transform.SetParent(InventorySlot_Parent.transform);
        }

        //Instantiate inventoryItemList based on the inventorySize
        for (int i = 0; i < inventorySize; i++)
        {
            if ((inventoryItemList.Count) < inventorySize)
            {
                InventoryItem item = new InventoryItem();

                inventoryItemList.Add(item);
            }
            else
            {
                break;
            }
        }

        setupInventory = true;
    }
    void DeleteSlotsInInventory()
    {
        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            if (inventorySlotList[i] != null)
            {
                Destroy(inventorySlotList[i].GetComponent<ItemSlot>().gameObject);
            }
        }
        inventorySlotList.Clear();

        while (inventoryDraggingParent.transform.childCount > 0)
        {
            DestroyImmediate(inventoryDraggingParent.transform.GetChild(0).GetComponent<DragDrop>().gameObject);
        }
        dragDropTempList.Clear();

        setupInventory = false;
    }
    public void UpdateInventoryDisplay()
    {
        //Set inventoryDraggingParent to having only 1 child
        while (inventoryDraggingParent.transform.childCount > 1)
        {
            DestroyImmediate(inventoryDraggingParent.transform.GetChild(inventoryDraggingParent.transform.childCount - 1).GetComponent<DragDrop>().gameObject);
        }

        //Check if inventorySlotList has elements
        if (inventorySlotList.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            if (inventoryItemList[i].itemName == Items.None)
            {
                //Set empty ItemSlot if inventoryItemList[i] is empty
                if (inventorySlotList[i] == null)
                {
                    return;
                }
                if (inventorySlotList[i].GetComponent<ItemSlot>() == null)
                {
                    return;
                }
                if (inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>() == null)
                {
                    return;
                }

                inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>().itemImage.sprite = emptySprite;
                inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>().amountText.text = "";
            }
            else
            {
                //Find correct item from _SO
                Item itemTemp = FindSO_Item(inventoryItemList[i]);

                if (inventorySlotList[i].GetComponent<ItemSlot>() == null)
                {
                    return;
                }
                if (inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>() == null)
                {
                    return;
                }

                //Insert data from this _SO item
                inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>().itemImage.sprite = itemTemp.itemSprite;

                for (int j = 0; j < SO_Item.itemList.Count; j++)
                {
                    if (inventoryItemList[i].itemName == SO_Item.itemList[j].itemName)
                    {
                        if (SO_Item.itemList[j].itemStackMax <= 1)
                        {
                            inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>().amountText.text = "";
                        }
                        else
                        {
                            inventorySlotList[i].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>().amountText.text = inventoryItemList[i].amount.ToString();
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


    void AddSlotToInventory()
    {
        //Add ItemSlot


        //Resize Frame


    } //
    void RemoveSlotFromInventory(Items itemName)
    {


    } //

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

    void RemoveItemFromInventory()
    {

    } //

    void SortInventoryItemsBy_SOPosition()
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

        PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;

        UpdateInventoryDisplay();
    }
    void SortInventoryItemsByInventoryPosition()
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

        PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;

        UpdateInventoryDisplay();
    }

    void UpdateInventoryItemListOrder()
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


    void IncreaseItemAmountHolding()
    {
        DragDrop dragDropTemp;
        itemIsSplitted = true;

        if (itemIsDragging)
        {
            dragDropTemp = FindDragDropOfDraggingParentLocation();
        }
        else
        {
            dragDropTemp = FindCorrectDragDropElementClicked();
        }

        Item itemTemp = FindCorrectSO_Item();

        if (dragDropTemp && itemAmountSelected < maxItemInSelectedStack)
        {
            itemAmountSelected += 1;
            CalculateItemAmountLeftBehind();

            DisplayCurrentItemAmountHolding(dragDropTemp);
        }

        UpdateDragDropTemp();
    }
    void DecreaseItemAmountHolding()
    {
        DragDrop dragDropTemp;
        itemIsSplitted = true;

        if (itemIsDragging)
        {
            dragDropTemp = FindDragDropOfDraggingParentLocation();
        }
        else
        {
            dragDropTemp = FindCorrectDragDropElementClicked();
        }

        if (dragDropTemp && itemAmountSelected > 1)
        {
            itemAmountSelected -= 1;
            CalculateItemAmountLeftBehind();

            DisplayCurrentItemAmountHolding(dragDropTemp);
        }

        UpdateDragDropTemp();
    }
    void ItemStack_PickOne()
    {
        DragDrop dragDropTemp = FindCorrectDragDropElementClicked();
        itemIsSplitted = true;

        if (dragDropTemp == null)
        {
            return;
        }

        if (dragDropTemp.amountText.text == "")
        {
            maxItemInSelectedStack = 0;
        }
        else
        {
            maxItemInSelectedStack = int.Parse(dragDropTemp.amountText.text);
        }

        if (dragDropTemp)
        {
            if (dragDropTemp.amountText.text == "" || dragDropTemp.amountText.text == "0")
            {
                itemAmountSelected = 0;
                CalculateItemAmountLeftBehind();

                return;
            }

            itemAmountSelected = 1;
            CalculateItemAmountLeftBehind();

            DisplayCurrentItemAmountHolding(dragDropTemp);
        }

        UpdateDragDropTemp();
    }
    void ItemStack_PickHalf()
    {
        DragDrop dragDropTemp = FindCorrectDragDropElementClicked();
        itemIsSplitted = true;

        if (dragDropTemp == null)
        {
            return;
        }

        if (dragDropTemp.amountText.text == "")
        {
            maxItemInSelectedStack = 0;
        }
        else
        {
            maxItemInSelectedStack = int.Parse(dragDropTemp.amountText.text);
        }

        if (dragDropTemp)
        {
            if (dragDropTemp.amountText.text == "" || dragDropTemp.amountText.text == "0")
            {
                itemAmountSelected = 0;
                CalculateItemAmountLeftBehind();

                return;
            }

            int temp = int.Parse(dragDropTemp.amountText.text);

            if (temp % 2 == 0)
            {
                itemAmountSelected = temp / 2;
            }
            else
            {
                itemAmountSelected = (temp + 1) / 2;
            }
            CalculateItemAmountLeftBehind();

            DisplayCurrentItemAmountHolding(dragDropTemp);
        }

        UpdateDragDropTemp();
    }
    void ItemStack_PickAll()
    {
        DragDrop dragDropTemp = FindCorrectDragDropElementClicked();
        itemIsSplitted = true;

        if (dragDropTemp == null)
        {
            return;
        }

        if (dragDropTemp.amountText.text == "")
        {
            maxItemInSelectedStack = 0;
        }
        else
        {
            maxItemInSelectedStack = int.Parse(dragDropTemp.amountText.text);
        }

        if (dragDropTemp)
        {
            if (dragDropTemp.amountText.text == "" || dragDropTemp.amountText.text == "0")
            {
                itemAmountSelected = 0;
                CalculateItemAmountLeftBehind();

                return;
            }

            itemAmountSelected = maxItemInSelectedStack;
            CalculateItemAmountLeftBehind();

            DisplayCurrentItemAmountHolding(dragDropTemp);
        }

        UpdateDragDropTemp();
    }

    DragDrop FindCorrectDragDropElementClicked()
    {
        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            if (inventorySlotList[i].GetComponent<ItemSlot>().gameObject.transform.childCount > 0)
            {
                int count = inventorySlotList[i].GetComponent<ItemSlot>().gameObject.transform.childCount;

                if(inventorySlotList[i].GetComponent<ItemSlot>() != null)
                {
                    if ((inventorySlotList[i].GetComponent<ItemSlot>().gameObject.transform.GetChild(count - 1).GetComponent<DragDrop>() != null))
                    {
                        if (inventorySlotList[i].GetComponent<ItemSlot>().gameObject.transform.GetChild(count - 1).GetComponent<DragDrop>().isClicked)
                        {
                            return inventorySlotList[i].GetComponent<ItemSlot>().gameObject.transform.GetChild(count - 1).GetComponent<DragDrop>();
                        }
                    }
                }
            }
        }

        return null;
    }
    DragDrop FindDragDropOfDraggingParentLocation()
    {
        if (inventoryDraggingParent.transform.childCount > 0)
        {
            if (inventoryDraggingParent.GetComponentInChildren<DragDrop>().isClicked)
            {
                return inventoryDraggingParent.GetComponentInChildren<DragDrop>();
            }
        }
        
        return null;
    }
    ItemSlot FindCorrectItemSlot()
    {
        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            if (inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>().isClicked)
            {
                return inventorySlotList[i].GetComponent<ItemSlot>();
            }
        }

        return null;
    }
    Item FindCorrectSO_Item()
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            for (int j = 0; j < SO_Item.itemList.Count; j++)
            {
                if (inventoryItemList[i].itemName == SO_Item.itemList[j].itemName)
                {
                    return SO_Item.itemList[j];
                }
            }
        }

        return null;
    }
    void DisplayCurrentItemAmountHolding(DragDrop dragDrop)
    {
        dragDrop.amountText.text = itemAmountSelected.ToString();
    }


    //--------------------


    public void CreateDragDropTemp(Transform parent, DragDrop dragDrop)
    {
        if (dragDrop == null)
        {
            return;
        }

        if (inventoryItemList[activeInventorySlotList_Index].itemName == Items.None)
        {
            selectedItemIsEmpty = true;

            return;
        }

        selectedItemIsEmpty = false;

        //Instantiate new dragDropTemp_Prefab
        dragDropTempList.Add(Instantiate(dragDropTemp_Prefab) as GameObject);
        dragDropTempList[dragDropTempList.Count - 1].transform.SetParent(parent);
        dragDropTempList[dragDropTempList.Count - 1].transform.position = parent.position;

        //Insert info
        if (inventorySlotList[activeInventorySlotList_Index].GetComponent<ItemSlot>() == null)
        {
            return;
        }
        if (inventorySlotList[activeInventorySlotList_Index].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>() == null)
        {
            return;
        }
        if (dragDropTempList[dragDropTempList.Count - 1].GetComponent<DragDrop>() == null)
        {
            return;
        }

        dragDropTempList[dragDropTempList.Count - 1].GetComponent<DragDrop>().itemImage.sprite = inventorySlotList[activeInventorySlotList_Index].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>().itemImage.sprite;
        dragDropTempList[dragDropTempList.Count - 1].GetComponent<DragDrop>().amountText.text = inventorySlotList[activeInventorySlotList_Index].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>().amountText.text;

        dragDrop.selectedFrameImage.gameObject.SetActive(false);

        dragDropTempList[dragDropTempList.Count - 1].GetComponent<DragDrop>().amountText.gameObject.SetActive(false);

        inventorySlotList[activeInventorySlotList_Index].GetComponent<ItemSlot>().GetComponentInChildren<DragDrop>().transform.SetAsLastSibling();
    }
    void CalculateItemAmountLeftBehind()
    {
        itemAmountLeftBehind = maxItemInSelectedStack - itemAmountSelected;
    }
    void UpdateDragDropTemp()
    {
        if (dragDropTempList.Count > 0)
        {
            dragDropTempList[0].GetComponent<DragDrop>().amountText.text = itemAmountLeftBehind.ToString();
        }
    }
    public void DeleteDragDropTemp(DragDrop dragDrop)
    {
        //Set inventoryDraggingParent to having only 1 child
        //while (inventoryDraggingParent.transform.childCount > 0)
        //{
        //    DestroyImmediate(inventoryDraggingParent.transform.GetChild(inventoryDraggingParent.transform.childCount - 1).GetComponent<DragDrop>().gameObject);
        //}

        //Delete from
        for (int i = 0; i < dragDropTempList.Count; i++)
        {
            if (dragDropTempList[i] != null)
            {
                if (dragDropTempList[i].GetComponent<DragDrop>() != null)
                {
                    dragDropTempList[i].GetComponent<DragDrop>().DeleteThisObject();
                }
            }
        }

        dragDropTempList.Clear();

        if (!selectedItemIsEmpty)
        {
            if (dragDrop != null)
            {
                dragDrop.selectedFrameImage.gameObject.SetActive(false);
            }
        }
    }


    //--------------------


    void OpenInventoryScreen()
    {
        if (!isOpen)
        {
            MainManager.instance.menuStates = MenuStates.InventoryMenu;

            SetupSlotsInInventory();
            UpdateInventoryDisplay();

            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            isOpen = true;
        }
        else if (isOpen)
        {
            CloseInventoryScreen();
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
                if (inventorySlotList[i].GetComponent<ItemSlot>() != null)
                {
                    if (inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>() != null)
                    {
                        inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>().selectedFrameImage.gameObject.SetActive(false);
                    }
                }
            }

            MainManager.instance.menuStates = MenuStates.None;
            PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;
            DeleteSlotsInInventory();
        }
    }
}

[Serializable]
public class InventoryItem
{
    public Items itemName;
    public int amount;
}

