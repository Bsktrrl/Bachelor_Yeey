using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    public static StorageManager instance { get; private set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
    }

    public Item_SO item_SO;

    [SerializeField] GameObject itemSlot_Prefab;
    [SerializeField] bool storageIsOpen;
    public bool sortIsActive;

    [Header("PlayerInventoryDisplay")]
    public Image itemSprite_Display;
    public TextMeshProUGUI itemName_Display;
    public TextMeshProUGUI itemDescription_Display;
    public List<InventoryItem> inventoryItemList = new List<InventoryItem>();

    #region
    [Header("Active")]
    public InventoryItem activeInventoryItem;
    public List<GameObject> activeSlotList = new List<GameObject>();
    public int activeSlotList_Index;
    public int activeInventoryList_Index;

    [Header("Target")]
    public InventoryItem targetInventoryItem;
    public List<GameObject> targetSlotList = new List<GameObject>();
    public int targetSlotList_Index;
    public int targetInventoryList_Index;


    [Header("DraggeableObject States")]
    public GameObject itemSlotDraggingParent;
    public bool itemIsClicked;
    public bool itemIsQuickClicked;
    public bool itemIsDragging;
    public bool itemIsSplitted;
    public bool selectedItemIsEmpty;

    public int itemAmountSelected;
    public int maxItemInSelectedStack;
    public int itemAmountLeftBehind;
    #endregion


    //-----


    #region Storage Box (2->)
    [Header("Storage Box")]
    public List<GameObject> StorageBoxItemSlotList = new List<GameObject>();
    [SerializeField] GameObject StorageBoxScreenUI;
    [SerializeField] GameObject StorageBoxItemSlot_Parent;
    [SerializeField] GameObject StorageBoxBG_Parent;
    public Inventories StorageBoxInventory;

    [SerializeField] Image storageImage;
    [SerializeField] Sprite smallChestSprite;
    [SerializeField] Sprite mediumChestSprite;

    [SerializeField] Button storageReverseButton;
    public bool storageReverseButton_State;

    public bool storageBoxIsOpen;
    public int storageBoxInventoryIndex;

    #endregion
    #region Player Inventory (1)
    [Header("Player Inventory")]
    int handSlots = 9;
    [SerializeField] GameObject PlayerHandItemSlot_Parent;

    public List<GameObject> PlayerInventoryItemSlotList = new List<GameObject>();
    [SerializeField] GameObject PlayerInventoryScreenUI;
    [SerializeField] GameObject PlayerInventoryItemSlot_Parent;
    [SerializeField] GameObject PlayerInventoryBG_Parent;
    public Inventories PlayerInventory;

    [SerializeField] Button PlayerInventoryReverseButton;
    public bool PlayerInventoryReverseButton_State;

    #endregion
    #region Player Selection Panel (0)
    [Header("Player Selection Panel")]
    [SerializeField] GameObject PlayerHandPanelScreenUI;
    [SerializeField] GameObject PlayerHandPanelItemSlot_Parent;

    #endregion

    #region Crafting Menu
    [Header("Crafting Menu")]
    [SerializeField] GameObject CraftingMenuScreenUI;
    #endregion


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
        //Make access to Events
        PlayerButtonManager.Tab_isPressedDown += OpenPlayerInventory;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;

        PlayerButtonManager.inventory_LeftMouse_isPressedDown += ItemStackPickAll;
        PlayerButtonManager.inventory_RightMouse_isPressedDown += ItemStack_PickOne;
        PlayerButtonManager.inventory_ScrollMouse_isPressedDown += ItemStack_PickHalf;
        PlayerButtonManager.inventory_Shift_and_RightMouse_isPressedDown += ItemStackPickAll;

        PlayerButtonManager.inventory_ScrollMouse_isRolledUP += IncreaseItemAmountHolding;
        PlayerButtonManager.inventory_ScrollMouse_isRolledDown += DecreaseItemAmountHolding;

        PlayerButtonManager.moveStackToStorageBox += QuickMoveItems;

        //Close all ScreenUI's
        PlayerHandPanelScreenUI.SetActive(true);

        PlayerInventoryScreenUI.SetActive(false);
        StorageBoxScreenUI.SetActive(false);
        CraftingMenuScreenUI.SetActive(false);

        //Reset PlayerDisplay 
        itemSprite_Display.sprite = item_SO.itemList[0].itemSprite;
        itemName_Display.text = "";
        itemDescription_Display.text = "";

        OpenPlayerInventory();
        CloseInventoryScreen();

        if (storageReverseButton_State)
        {
            storageReverseButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            storageReverseButton.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1);
        }

        if (PlayerInventoryReverseButton_State)
        {
            PlayerInventoryReverseButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            PlayerInventoryReverseButton.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1);
        }

        HandManager.instance.UpdateSlotInfo();
    }
    private void Update()
    {
        //Keep the HandPanel.rotation under control
        for (int i = 0; i < PlayerHandItemSlot_Parent.transform.childCount; i++)
        {
            PlayerHandItemSlot_Parent.transform.GetChild(i).GetComponent<ItemSlot_N>().gameObject.GetComponent<RectTransform>().rotation = Quaternion.identity;
        }
    }


    //-------------------- Construction


    public void SetupStorageScreens(int index)
    {
        //Open Crafting Screen
        CraftingManager.instance.OpenInventoryScreen();
        CraftingMenuScreenUI.SetActive(true);

        PlayerHandPanelScreenUI.SetActive(true);

        MainManager.instance.menuStates = MenuStates.InventoryMenu;


        //-----


        //Normal storage Box (index = 1->)
        #region
        if (index > 0)
        {
            //Open Player Inventory as well
            OpenPlayerInventory();

            StorageBoxScreenUI.SetActive(true);
            storageBoxIsOpen = true;
            storageBoxInventoryIndex = index;

            StorageBoxInventory = InventoryManager.instance.inventories[index];
            InventoryManager.instance.inventories[index].isOpen = true;

            ConstructStorage(StorageBoxInventory, StorageBoxItemSlot_Parent, StorageBoxItemSlotList, index);

            if (StorageBoxItemSlotList.Count <= 5)
            {
                storageImage.sprite = smallChestSprite;
            }
            else if (StorageBoxItemSlotList.Count <= 15)
            {
                storageImage.sprite = mediumChestSprite;
            }
        }
        #endregion

        //Player Inventory (index <= 0)
        #region
        else
        {
            if (PlayerInventory == null || PlayerInventoryItemSlot_Parent == null || PlayerInventoryItemSlotList == null
                || InventoryManager.instance == null || InventoryManager.instance.inventories == null
                || PlayerInventoryScreenUI == null || index > 0)
            {
                return;
            }

            PlayerInventoryScreenUI.SetActive(true);
            PlayerInventory = InventoryManager.instance.inventories[index];
            InventoryManager.instance.inventories[index].isOpen = true;

            ConstructStorage(PlayerInventory, PlayerInventoryItemSlot_Parent, PlayerInventoryItemSlotList, index);
        }
        #endregion
    }

    void ConstructStorage(Inventories inventory, GameObject parent, List<GameObject> itemSlotList, int index)
    {
        if (index == 0)
        {
            int iteration = 0;

            if (PlayerHandItemSlot_Parent.transform.childCount > 0)
            {
                iteration = handSlots;
            }

            //Instantiate new itemSlots
            while (itemSlotList.Count > 9)
            {
                itemSlotList.RemoveAt(itemSlotList.Count - 1);
            }

            for (int i = iteration; i < inventory.inventorySize; i++)
            {
                itemSlotList.Add(Instantiate(itemSlot_Prefab) as GameObject);

                if (i < handSlots)
                {
                    itemSlotList[itemSlotList.Count - 1].transform.SetParent(PlayerHandItemSlot_Parent.transform);
                }
                else
                {
                    itemSlotList[itemSlotList.Count - 1].transform.SetParent(parent.transform);
                }

                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().slotIndexInInventory = itemSlotList.Count - 1;
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().itemInThisSlot = inventory.itemList[itemSlotList.Count - 1];
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().itemInThisSlotFromInventory = index;

                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().ItemSlotSettings(itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().itemInThisSlot);
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().UpdateSlider();
            }
        }
        else
        {
            //Instantiate new itemSlots
            itemSlotList.Clear();
            for (int i = 0; i < inventory.inventorySize; i++)
            {
                itemSlotList.Add(Instantiate(itemSlot_Prefab) as GameObject);

                itemSlotList[itemSlotList.Count - 1].transform.SetParent(parent.transform);
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().slotIndexInInventory = itemSlotList.Count - 1;
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().itemInThisSlot = inventory.itemList[itemSlotList.Count - 1];
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().itemInThisSlotFromInventory = index;

                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().ItemSlotSettings(itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().itemInThisSlot);
                itemSlotList[itemSlotList.Count - 1].GetComponent<ItemSlot_N>().UpdateSlider();
            }
        }

        //Make correct ScreenSize
        AdjustPanelSize(parent, inventory.inventorySize, index);

        //Set sprites in the itemSlots
        for (int i = 0; i < itemSlotList.Count; i++)
        {
            for (int j = 0; j < item_SO.itemList.Count; j++)
            {
                if (itemSlotList[i].GetComponent<ItemSlot_N>() == null || item_SO == null || item_SO.itemList == null)
                {

                }
                else if (itemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == item_SO.itemList[j].itemName)
                {
                    itemSlotList[i].GetComponent<ItemSlot_N>().draggeableSlotScript.itemImage.sprite = item_SO.itemList[j].itemSprite;
                    itemSlotList[i].GetComponent<ItemSlot_N>().draggeableSlotScript.ghostImage.sprite = item_SO.itemList[j].itemSprite;
                }
            }
        }
    }
    void AdjustPanelSize(GameObject parent, int storageSize, int index)
    {
        //Set standard size
        if (index == 0)
        {
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(378, 215);
            PlayerInventoryBG_Parent.GetComponent<RectTransform>().sizeDelta = new Vector2(378, 215);

            //Add based on amount of storageSize
            //Start on 9 because of SelectionScreen
            for (int i = 9; i < storageSize; i += 5)
            {
                parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 82);
                PlayerInventoryBG_Parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 72);
            }
        }
        else if (index > 0)
        {
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(378, 115);
            StorageBoxBG_Parent.GetComponent<RectTransform>().sizeDelta = new Vector2(378, 115);

            //Add based on amount of storageSize
            for (int i = 0; i < storageSize; i += 5)
            {
                parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 82);
                StorageBoxBG_Parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 72);
            }
        }
    }


    //-------------------- Add/Remove Items


    public void AddItem(Items itemName, int amount)
    {
        List<InventoryItem> inventory = PlayerInventory.itemList;
        int amountTemp = amount;

        //Find item in ItemList
        Item item = new Item();
        for (int i = 0; i < item_SO.itemList.Count; i++)
        {
            if (item_SO.itemList[i].itemName == itemName)
            {
                item = item_SO.itemList[i];

                break;
            }
        }

        //Find slot of this type
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemName == itemName)
            {
                //Check if Slot can take all amount of the item
                if (inventory[i].amount + amountTemp <= item.itemStackMax)
                {
                    inventory[i].amount += amountTemp;

                    return;
                }
                else
                {
                    int temp = item.itemStackMax - inventory[i].amount;
                    inventory[i].amount += temp;
                    amountTemp -= temp;
                }
            }
        }

        //Search find Empty Slot
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemName == Items.None)
            {
                if (amountTemp <= item.itemStackMax)
                {
                    inventory[i].itemName = itemName;
                    inventory[i].amount += amountTemp;
                    inventory[i].hp = item.HP;

                    PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().ItemSlotSettings(inventory[i]);

                    return;
                }
                else
                {
                    inventory[i].itemName = itemName;
                    inventory[i].amount += item.itemStackMax;
                    amountTemp -= item.itemStackMax;
                }
            }
        }

        //If there isn't room for the item in the Inventory
        if (amountTemp > 0)
        {
            //Add object to the World

            print("The inventory is full");
        }

        InventoryManager.instance.UpdateInventory(PlayerInventory);
    }
    public void RemoveLastItem(Items itemName)
    {
        for (int i = 0; i < InventoryManager.instance.inventories.Count; i++)
        {
            if (InventoryManager.instance.inventories[i].isOpen)
            {
                for (int j = InventoryManager.instance.inventories[i].itemList.Count - 1; j >= 0; j--)
                {
                    if (InventoryManager.instance.inventories[i].itemList[j].itemName == itemName
                        && InventoryManager.instance.inventories[i].itemList[j].amount > 0)
                    {
                        InventoryManager.instance.inventories[i].itemList[j].amount -= 1;

                        if (InventoryManager.instance.inventories[i].itemList[j].amount <= 0)
                        {
                            InventoryManager.instance.inventories[i].itemList[j].itemName = Items.None;
                            InventoryManager.instance.inventories[i].itemList[j].amount = 0;
                        }

                        break;
                    }
                }
            }
        }
    }
    public void RemoveItemFromInventory(int item)
    {
        InventoryManager.instance.inventories[0].itemList[item].itemName = Items.None;
        InventoryManager.instance.inventories[0].itemList[item].amount = 0;
        InventoryManager.instance.inventories[0].itemList[item].hp = 0;
    }


    //-------------------- Item Splitting to DragDrop


    void ItemStackPickAll()
    {
        if (itemIsClicked)
        {
            //Set Premisses
            maxItemInSelectedStack = activeInventoryItem.amount;
            itemAmountSelected = maxItemInSelectedStack;
            itemAmountLeftBehind = maxItemInSelectedStack - itemAmountSelected;

            //Update itemInThisSlot.amount
            activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot.amount = itemAmountSelected;
        }
    }
    void ItemStack_PickOne()
    {
        if (activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>() == null)
        {
            return;
        }

        //Set stats
        itemIsSplitted = true;

        //Set Premisses
        maxItemInSelectedStack = activeInventoryItem.amount;
        itemAmountSelected = 1;
        itemAmountLeftBehind = maxItemInSelectedStack - itemAmountSelected;

        //Update itemInThisSlot.amount
        activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot.amount = itemAmountSelected;
    }
    void ItemStack_PickHalf()
    {
        //Set stats
        itemIsSplitted = true;

        //Set Premisses
        maxItemInSelectedStack = activeInventoryItem.amount;
        if (maxItemInSelectedStack % 2 == 0)
            itemAmountSelected = maxItemInSelectedStack / 2;
        else
            itemAmountSelected = (maxItemInSelectedStack + 1) / 2;
        itemAmountLeftBehind = maxItemInSelectedStack - itemAmountSelected;

        //Update itemInThisSlot.amount
        activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot.amount = itemAmountSelected;
    }
    void IncreaseItemAmountHolding()
    {
        //Set stats
        itemIsSplitted = true;

        //Set Premisses
        if (itemAmountSelected < maxItemInSelectedStack && itemAmountLeftBehind > 0)
        {
            itemAmountSelected += 1;
            itemAmountLeftBehind -= 1;
        }

        //Update itemInThisSlot.amount
        activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot.amount = itemAmountSelected;
    }
    void DecreaseItemAmountHolding()
    {
        //Set stats
        itemIsSplitted = true;

        //Set Premisses
        if (itemAmountLeftBehind < maxItemInSelectedStack && itemAmountSelected > 1)
        {
            itemAmountSelected -= 1;
            itemAmountLeftBehind += 1;
        }

        //Update itemInThisSlot.amount
        activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot.amount = itemAmountSelected;
    }

    void QuickMoveItems()
    {
        itemIsQuickClicked = true;

        //Set Premisses
        maxItemInSelectedStack = activeInventoryItem.amount;

        //"From Inventory to StorageBox" or "From Inventory to Inventory"
        if (activeInventoryList_Index <= 0)
        {
            //Return item to StorageBox
            if (StorageBoxScreenUI.activeInHierarchy)
            {
                //print("1. Return item from Inventory to StorageBox");

                MoveToStorageBox();
            }

            //Return to PlayerInventory
            else
            {
                //Return to Large Inventory
                if (activeSlotList_Index < 9)
                {
                    //print("2. Return from Hold Inventory to Large Inventory");

                    MoveToLargeInventory();
                }

                //Return to HoldPanel
                else
                {
                    //print("3. Return from Large Inventory to Hold Inventory");

                    MoveToHoldInventory();
                }
            }
        }

        //From StorageBox to Large Inventory
        else
        {
            //print("4. Return from StorageBox to Large Inventory");

            MoveFromStorageBox();
        }

        PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;
        PlayerButtonManager.instance.buttonClickedState = ButtonClickedState.None;


        itemIsDragging = false;
    }
    void MoveToStorageBox()
    {
        //Check available Spaces in the Hold Inventory
        for (int i = 0; i < StorageBoxItemSlotList.Count; i++)
        {
            if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == Items.None)
            {
                //print("1. Success");

                InventoryItem tempName = activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot;

                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot = StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot;
                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().UpdateSlider();

                StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot = tempName;
                StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().UpdateSlider();

                return;
            }
        }
    }
    void MoveToLargeInventory()
    {
        //Check available Spaces in the Hold Inventory
        for (int i = 9; i < PlayerInventoryItemSlotList.Count; i++)
        {
            if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == Items.None)
            {
                //print("1. Success");

                InventoryItem tempName = activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot;

                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot = PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot;
                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().UpdateSlider();

                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot = tempName;
                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().UpdateSlider();

                return;
            }
        }
    }
    void MoveToHoldInventory()
    {
        //Check available Spaces in the Hold Inventory
        for (int i = 0; i < 9; i++)
        {
            if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>() == null)
            {
                return;
            }

            if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == Items.None)
            {
                //print("2. Success");

                InventoryItem tempName = activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot;

                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot = PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot;
                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().UpdateSlider();

                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot = tempName;
                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().UpdateSlider();

                return;
            }
        }
    }
    void MoveFromStorageBox()
    {
        //Check available Spaces in the Hold Inventory
        for (int i = 9; i < PlayerInventoryItemSlotList.Count; i++)
        {
            if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == Items.None)
            {
                //print("1. Success");

                InventoryItem tempName = activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot;

                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().itemInThisSlot = PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot;
                activeSlotList[activeSlotList_Index].GetComponent<ItemSlot_N>().UpdateSlider();

                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot = tempName;
                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().UpdateSlider();

                return;
            }
        }
    }


    //-------------------- Sort


    public void SortInventory()
    {
        sortIsActive = true;
        SoundManager.instance.Playmenu_SortInventory_Clip();

        //Bubble Sort - Get itemsSlots in correct order
        for (int WholeList = 0; WholeList < 30; WholeList++)
        {
            for (int i = 0; i < PlayerInventoryItemSlotList.Count; i++)
            {
                for (int j = 0; j < item_SO.itemList.Count; j++)
                {
                    if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == item_SO.itemList[j].itemName)
                    {
                        PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot_ItemIndex = j;

                        break;
                    }
                }
            }
            int length = PlayerInventoryItemSlotList.Count;

            InventoryItem temp = PlayerInventoryItemSlotList[9].GetComponent<ItemSlot_N>().itemInThisSlot;

            for (int i = 9; i < length; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot_ItemIndex > PlayerInventoryItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot_ItemIndex)
                    {
                        temp = PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot;

                        PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot = PlayerInventoryItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot;

                        PlayerInventoryItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot = temp;
                    }
                }
            }

            //Merge all items into available slots
            for (int i = 0; i < PlayerInventoryItemSlotList.Count - 1; i++)
            {
                //If slot has something
                if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName != Items.None)
                {
                    //If both slots are the same
                    if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == PlayerInventoryItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.itemName)
                    {
                        //Check StackMax
                        int stackMax = StackMax(PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName);

                        //If next slot has amount under stackMax
                        if (PlayerInventoryItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount < stackMax)
                        {
                            int first = PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount;
                            int second = PlayerInventoryItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount;

                            if ((first + second) <= stackMax)
                            {
                                PlayerInventoryItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount += first;

                                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                            }
                            else
                            {
                                int dif = stackMax - second;

                                PlayerInventoryItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount += dif;
                                PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= dif;

                                if (PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
                                {
                                    PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                                    PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Update Sliders
        for (int i = 0; i < PlayerInventoryItemSlotList.Count; i++)
        {
            PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().UpdateSlider();
        }

        sortIsActive = false;
    }
    public void ReverseInventory()
    {
        if (PlayerInventoryReverseButton_State)
        {
            PlayerInventoryReverseButton_State = false;
        }
        else
        {
            PlayerInventoryReverseButton_State = true;
        }

        if (PlayerInventoryReverseButton_State)
        {
            PlayerInventoryReverseButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            PlayerInventoryReverseButton.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1);
        }

        PlayerInventoryItemSlotList.Reverse();

        SortInventory();
    }
    public void SortStorageBox()
    {
        sortIsActive = true;
        SoundManager.instance.Playmenu_SortInventory_Clip();

        //Bubble Sort - Get itemsSlots in correct order
        for (int WholeList = 0; WholeList < 30; WholeList++)
        {
            for (int i = 0; i < StorageBoxItemSlotList.Count; i++)
            {
                for (int j = 0; j < item_SO.itemList.Count; j++)
                {
                    if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == item_SO.itemList[j].itemName)
                    {
                        StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot_ItemIndex = j;

                        break;
                    }
                }
            }
            int length = StorageBoxItemSlotList.Count;

            InventoryItem temp = StorageBoxItemSlotList[0].GetComponent<ItemSlot_N>().itemInThisSlot;

            for (int i = 0; i < length; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot_ItemIndex > StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot_ItemIndex)
                    {
                        temp = StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot;

                        StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot = StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot;

                        StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot = temp;
                    }
                }
            }

            //Merge all items into available slots
            for (int i = 0; i < StorageBoxItemSlotList.Count - 1; i++)
            {
                //If slot has something
                if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName != Items.None)
                {
                    //If both slots are the same
                    if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == StorageBoxItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.itemName)
                    {
                        //Check StackMax
                        int stackMax = StackMax(StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName);

                        //If next slot has amount under stackMax
                        if (StorageBoxItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount < stackMax)
                        {
                            int first = StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount;
                            int second = StorageBoxItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount;

                            if ((first + second) <= stackMax)
                            {
                                StorageBoxItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount += first;

                                StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                                StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                            }
                            else
                            {
                                int dif = stackMax - second;

                                StorageBoxItemSlotList[i + 1].GetComponent<ItemSlot_N>().itemInThisSlot.amount += dif;
                                StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= dif;

                                if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
                                {
                                    StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                                    StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Update Sliders
        for (int i = 0; i < StorageBoxItemSlotList.Count; i++)
        {
            StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().UpdateSlider();
        }

        sortIsActive = false;
    }
    public void ReverseStorageBox()
    {
        if (storageReverseButton_State)
        {
            storageReverseButton_State = false;
        }
        else
        {
            storageReverseButton_State = true;
        }

        if (storageReverseButton_State)
        {
            storageReverseButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            storageReverseButton.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1);
        }

        StorageBoxItemSlotList.Reverse();

        SortStorageBox();
    }
    public void FillStorageBoxNew()
    {
        //Fill available
        for (int i = 9; i < PlayerInventoryItemSlotList.Count; i++)
        {
            for (int j = 0; j < StorageBoxItemSlotList.Count; j++)
            {
                if (StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName
                    && StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.itemName != Items.None)
                {
                    int stackMax = 0;
                    for (int k = 0; k < item_SO.itemList.Count; k++)
                    {
                        if (item_SO.itemList[k].itemName == StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.itemName)
                        {
                            stackMax = item_SO.itemList[k].itemStackMax;
                        }
                    }

                    //If transfer works
                    if (StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.amount + PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= stackMax)
                    {
                        StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.amount += PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount;

                        PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
                        PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
                    }
                    //If not, only transfer what's possible
                    else
                    {
                        int temp = stackMax - StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.amount;

                        StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.amount += temp;
                        PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= temp;
                    }
                }
            }
        }

        //Fill Empty
        //for (int i = 0; i < StorageBoxItemSlotList.Count; i++)
        //{
        //    if (StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == Items.None)
        //    {
        //        for (int j = 0; j < StorageBoxItemSlotList.Count; j++)
        //        {
        //            for (int k = 9; k < PlayerInventoryItemSlotList.Count; k++)
        //            {
        //                int stackMaxTwo = 0;
        //                for (int l = 0; l < item_SO.itemList.Count; l++)
        //                {
        //                    if (item_SO.itemList[l].itemName == StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.itemName)
        //                    {
        //                        stackMaxTwo = item_SO.itemList[l].itemStackMax;
        //                    }
        //                }

        //                print("stackMaxTwo: " + stackMaxTwo);

        //                if (StorageBoxItemSlotList[j].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.itemName)
        //                {
        //                    //If transfer works
        //                    if (PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= stackMaxTwo)
        //                    {
        //                        StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount += PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.amount;

        //                        PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
        //                        PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
        //                        break;
        //                    }
        //                    //If not, only transfer what's possible
        //                    else
        //                    {
        //                        StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.amount += stackMaxTwo;
        //                        PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.amount -= stackMaxTwo;

        //                        if (PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.amount <= 0)
        //                        {
        //                            PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.itemName = Items.None;
        //                            PlayerInventoryItemSlotList[k].GetComponent<ItemSlot_N>().itemInThisSlot.amount = 0;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            break;
        //        }
        //    }
        //}
    }


    //-------------------- Functions

    public int StackMax(Items itemName)
    {
        for (int i = 0; i < item_SO.itemList.Count; i++)
        {
            if (item_SO.itemList[i].itemName == itemName)
            {
                return item_SO.itemList[i].itemStackMax;
            }
        }

        return 0;
    }


    //-------------------- Open/Close Inventory

    void OpenPlayerInventory()
    {
        if (storageIsOpen)
        {
            CloseInventoryScreen();
        }
        else
        {
            storageIsOpen = true;

            Cursor.lockState = CursorLockMode.None;

            SetupStorageScreens(0);
        }
    }
    void CloseInventoryScreen()
    {
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            //Destroy all ItemObjects
            #region
            while (StorageBoxItemSlot_Parent.transform.childCount > 0)
            {
                DestroyImmediate(StorageBoxItemSlot_Parent.transform.GetChild(0).gameObject);
            }
            while (PlayerInventoryItemSlot_Parent.transform.childCount > 0)
            {
                DestroyImmediate(PlayerInventoryItemSlot_Parent.transform.GetChild(0).gameObject);
            }

            StorageBoxItemSlotList.Clear();
            while (PlayerHandItemSlot_Parent.transform.childCount > 9)
            {
                DestroyImmediate(PlayerHandItemSlot_Parent.transform.GetChild(0).gameObject);
            }

            #endregion

            storageIsOpen = false;
            storageBoxIsOpen = true;

            for (int i = 0; i < InventoryManager.instance.inventories.Count; i++)
            {
                InventoryManager.instance.inventories[i].isOpen = false;
            }

            PlayerInventoryScreenUI.SetActive(false);
            StorageBoxScreenUI.SetActive(false);
            CraftingManager.instance.CloseInventoryScreen();

            Cursor.lockState = CursorLockMode.Locked;
            MainManager.instance.menuStates = MenuStates.None;
        }
    }
}
