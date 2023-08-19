using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    public static StorageManager instance { get; private set; } //Singleton

    public Item_SO item_SO;

    [SerializeField] GameObject itemSlot_Prefab;
    [SerializeField] bool storageIsOpen;

    [Header("PlayerInventoryDisplay")]
    public Image itemSprite_Display;
    public TextMeshProUGUI itemName_Display;
    public TextMeshProUGUI itemDescription_Display;

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
    public bool itemIsDragging;
    public bool itemIsSplitted;
    public bool selectedItemIsEmpty;

    public int itemAmountSelected;
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
    [SerializeField] Sprite storageSprite;

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

    #endregion
    #region Player Selection Panel (0)
    [Header("Player Selection Panel")]
    [SerializeField] GameObject PlayerSelectionPanelScreenUI;
    [SerializeField] GameObject PlayerSelectionPanelItemSlot_Parent;

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

        //Close all ScreenUI's
        PlayerSelectionPanelScreenUI.SetActive(true);

        PlayerInventoryScreenUI.SetActive(false);
        StorageBoxScreenUI.SetActive(false);
        CraftingMenuScreenUI.SetActive(false);

        //Reset PlayerDisplay 
        itemSprite_Display.sprite = item_SO.itemList[0].itemSprite;
        itemName_Display.text = "";
        itemDescription_Display.text = "";

    //Setup static Assets
    storageImage.sprite = storageSprite;

        OpenPlayerInventory();
        CloseInventoryScreen();
    }
    private void Update()
    {
        //Keep the HandPanel.rotation under control
        for (int i = 0; i < PlayerHandItemSlot_Parent.transform.childCount; i++)
        {
            PlayerHandItemSlot_Parent.transform.GetChild(i).GetComponent<ItemSlot_N>().gameObject.GetComponent<RectTransform>().rotation = Quaternion.identity;
        }
    }


    //--------------------


    public void SetupStorageScreens(int index)
    {
        //Open Crafting Screen
        CraftingManager.instance.OpenInventoryScreen();
        CraftingMenuScreenUI.SetActive(true);

        PlayerSelectionPanelScreenUI.SetActive(true);

        MainManager.instance.menuStates = MenuStates.InventoryMenu;


        //-----


        //Normal storage Box (index = 1->)
        #region
        if (index > 0)
        {
            //Open Player Inventory as well
            OpenPlayerInventory();

            StorageBoxScreenUI.SetActive(true);

            StorageBoxInventory = InventoryManager.instance.inventories[index];
            InventoryManager.instance.inventories[index].isOpen = true;

            ConstructStorage(StorageBoxInventory, StorageBoxItemSlot_Parent, StorageBoxItemSlotList, index);
        }
        #endregion

        //Player Inventory (index <= 0)
        #region
        else
        {
            PlayerInventoryScreenUI.SetActive(true);
            PlayerInventory = InventoryManager.instance.inventories[index];
            InventoryManager.instance.inventories[index].isOpen = true;

            ConstructStorage(PlayerInventory, PlayerInventoryItemSlot_Parent, PlayerInventoryItemSlotList, index);
        }
        #endregion
    }


    //--------------------


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
            }
        }

        //Make correct ScreenSize
        AdjustPanelSize(parent, inventory.inventorySize, index);

        //Set sprites in the itemSlots
        for (int i = 0; i < itemSlotList.Count; i++)
        {
            for (int j = 0; j < item_SO.itemList.Count; j++)
            {
                if (itemSlotList[i].GetComponent<ItemSlot_N>().itemInThisSlot.itemName == item_SO.itemList[j].itemName)
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


    //--------------------


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

    //--------------------


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
