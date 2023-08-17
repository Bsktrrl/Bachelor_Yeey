using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance { get; set; } //Singleton

    [Header("Main Screen")]
    [SerializeField] GameObject craftingMenu;
    List<Item> SO_itemList = new List<Item>();

    [Header("Overview Screen")]
    [SerializeField] ItemCategory_SO itemCategory_SO;

    [SerializeField] GameObject overviewScreen;
    [SerializeField] GameObject overviewGridLayoutGroup;
    [SerializeField] GameObject categoryButton_Prefab;

    public List<ItemCategory> categoryItemsList = new List<ItemCategory>();
    public List<GameObject> categoryButtonPrefabList = new List<GameObject>();

    [Header("Selection Screen")]
    [SerializeField] GameObject selectionScreen;
    [SerializeField] Image categorySelectedImage;
    [SerializeField] TextMeshProUGUI categorySelectedName;

    [SerializeField] GameObject selectionButton_Prefab;
    [SerializeField] GameObject selectionSubGridLayoutGroup_Parent;
    [SerializeField] GameObject selectionSubGridLayoutGroup_Prefab;

    public List<bool> selectionSubActiveList = new List<bool>();
    public List<GameObject> selectionSubGridLayoutGroupList = new List<GameObject>();
    public List<GameObject> selectionButtonPrefabList = new List<GameObject>();

    [Header("Crafting Screen")]
    public GameObject craftingScreen;

    [SerializeField] Image categoryCraftingImage;
    [SerializeField] TextMeshProUGUI categoryCraftingName;
    [SerializeField] TextMeshProUGUI categoryCraftingDescription;

    [SerializeField] GameObject requirementGridLayoutGroup;
    [SerializeField] GameObject requirement_Prefab;
    public List<GameObject> requirementPrefabList = new List<GameObject>();
    public bool totalRequirementMet;

    [Header("Other")]
    public bool craftingScreen_isOpen;
    public ItemCategories activeCategory;
    public Item itemSelected;


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
        PlayerButtonManager.Tab_isPressedDown += OpenInventoryScreen;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;

        SO_itemList = InventorySystem.instance.SO_Item.itemList;

        craftingMenu.SetActive(false);
        overviewScreen.SetActive(false);
        selectionScreen.SetActive(false);
        craftingScreen.SetActive(false);

        SetupItemCategoryList();

        //Set the first useable Category in the itemCategory_SO.ItemCategoryList to be active by default
        activeCategory = itemCategory_SO.ItemCategoryList[1].categoryName;
        SetupSelectionScreen();
    }
    private void Update()
    {
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            CheckForRequiermentsMet();
        }
    }


    //--------------------


    //Overview Screen
    void SetupItemCategoryList()
    {
        //Reset overviewScreen
        categoryButtonPrefabList.Clear();

        //Destroy all Children of overviewGridLayoutGroup to prepare for reset
        while (overviewGridLayoutGroup.transform.childCount > 0)
        {
            DestroyImmediate(overviewGridLayoutGroup.transform.GetChild(0).gameObject);
        }

        //Reset Panel Size
        overviewScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);

        //Instantiate CategoryButtons
        for (int i = 1; i < itemCategory_SO.ItemCategoryList.Count; i++)
        {
            categoryButtonPrefabList.Add(Instantiate(categoryButton_Prefab) as GameObject);
            categoryButtonPrefabList[categoryButtonPrefabList.Count - 1].transform.SetParent(overviewGridLayoutGroup.transform);
            categoryButtonPrefabList[categoryButtonPrefabList.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = itemCategory_SO.ItemCategoryList[i].categorySprite;

            categoryButtonPrefabList[categoryButtonPrefabList.Count - 1].GetComponent<CategoryButton>().categoryType = itemCategory_SO.ItemCategoryList[i].categoryName;

            //Adjust Frame
            overviewScreen.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 85);
        }
    }


    //Selection Screen
    public void SetupSelectionScreen()
    {
        //CategoryDisplay
        categorySelectedImage.sprite = FindActiveCategoryType().categorySprite;
        categorySelectedName.text = FindActiveCategoryType().categoryName.ToString();

        //Reset Panel Size
        selectionScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 110);

        //Instantitate InstantiateSubGridLayoutGroup
        UpdateSelectionSubActiveList();
        InstantiateSubGridLayoutGroup();
        InstantiateSelectionButton_Prefab();
    }
    void UpdateSelectionSubActiveList()
    {
        selectionSubActiveList.Clear();

        //Build List
        for (int i = 1; i < itemCategory_SO.ItemCategoryList.Count; i++)
        {
            if (itemCategory_SO.ItemCategoryList[i].categoryName == activeCategory)
            {
                for (int j = 0; j < itemCategory_SO.ItemCategoryList[i].subCategoryName.Count; j++)
                {
                    selectionSubActiveList.Add(false);
                }

                break;
            }
        }

        //Turn available subCategoryies on
        for (int k = 1; k < SO_itemList.Count; k++)
        {
            for (int i = 1; i < itemCategory_SO.ItemCategoryList.Count; i++)
            {
                if (itemCategory_SO.ItemCategoryList[i].categoryName == activeCategory)
                {
                    for (int j = 0; j < itemCategory_SO.ItemCategoryList[i].subCategoryName.Count; j++)
                    {
                        if (SO_itemList[k].subCategoryName == itemCategory_SO.ItemCategoryList[i].subCategoryName[j])
                        {
                            selectionSubActiveList[j] = true;
                        }
                    }

                    break;
                }
            }
        }
    }
    public void InstantiateSubGridLayoutGroup()
    {
        //Prepare for reset
        selectionSubGridLayoutGroupList.Clear();
        while (selectionSubGridLayoutGroup_Parent.transform.childCount > 0)
        {
            DestroyImmediate(selectionSubGridLayoutGroup_Parent.transform.GetChild(0).gameObject);
        }

        //instantiate selectionSubGridLayoutGroup
        for (int i = 0; i < selectionSubActiveList.Count; i++)
        {
            if (selectionSubActiveList[i])
            {
                selectionSubGridLayoutGroupList.Add(Instantiate(selectionSubGridLayoutGroup_Prefab) as GameObject);
                selectionSubGridLayoutGroupList[selectionSubGridLayoutGroupList.Count - 1].transform.SetParent(selectionSubGridLayoutGroup_Parent.transform);

                //Set Name
                for (int j = 0; j < itemCategory_SO.ItemCategoryList.Count; j++)
                {
                    if (itemCategory_SO.ItemCategoryList[j].categoryName == activeCategory)
                    {
                        for (int k = 0; k < itemCategory_SO.ItemCategoryList[j].subCategoryName.Count; k++)
                        {
                            selectionSubGridLayoutGroupList[selectionSubGridLayoutGroupList.Count - 1].GetComponent<SelectionSubPanel>().panelName = itemCategory_SO.ItemCategoryList[j].subCategoryName[i];
                            selectionSubGridLayoutGroupList[selectionSubGridLayoutGroupList.Count - 1].GetComponent<SelectionSubPanel>().SetDisplay();

                            break;
                        }
                    }
                }

                //Adjust Frame
                selectionScreen.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 67);
            }
        }
    }
    void InstantiateSelectionButton_Prefab()
    {
        selectionButtonPrefabList.Clear();

        int ItemCategoryListIndex = 0;

        for (int i = 0; i < itemCategory_SO.ItemCategoryList.Count; i++)
        {
            if (itemCategory_SO.ItemCategoryList[i].categoryName == activeCategory)
            {
                ItemCategoryListIndex = i;

                break;
            }
        }

        //Instantiate SelectionButton_Prefab
        for (int i = 0; i < selectionSubGridLayoutGroupList.Count; i++)
        {
            //Find amount of items
            for (int j = 0; j < SO_itemList.Count; j++)
            {
                if (SO_itemList[j].isActive
                    && SO_itemList[j].categoryName == activeCategory
                    && SO_itemList[j].subCategoryName == selectionSubGridLayoutGroupList[i].GetComponent<SelectionSubPanel>().panelName)
                {
                    selectionButtonPrefabList.Add(Instantiate(selectionButton_Prefab) as GameObject);
                    selectionButtonPrefabList[selectionButtonPrefabList.Count - 1].transform.SetParent(selectionSubGridLayoutGroupList[i].transform);

                    selectionButtonPrefabList[selectionButtonPrefabList.Count - 1].GetComponent<SelectionSubButtonPrefab>().item = SO_itemList[j];
                    selectionButtonPrefabList[selectionButtonPrefabList.Count - 1].GetComponent<SelectionSubButtonPrefab>().SetDisplay();
                }
            }
        }
    }


    //Crafting Screen
    public void SetupCraftingScreen(Item item)
    {
        //Prepare for reset
        requirementPrefabList.Clear();
        while (requirementGridLayoutGroup.transform.childCount > 0)
        {
            DestroyImmediate(requirementGridLayoutGroup.transform.GetChild(0).gameObject);
        }

        //Reset Panel Size
        craftingScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(270, 220);

        categoryCraftingImage.sprite = item.itemSprite;
        categoryCraftingName.text = item.itemName.ToString();
        categoryCraftingDescription.text = item.itemDescription;

        InstantiateCraftingRequirementPrefabs(item);
    }
    public void InstantiateCraftingRequirementPrefabs(Item item)
    {
        for (int i = 0; i < item.craftingRequirements.Count; i++)
        {
            requirementPrefabList.Add(Instantiate(requirement_Prefab) as GameObject);
            requirementPrefabList[requirementPrefabList.Count - 1].transform.SetParent(requirementGridLayoutGroup.transform);
            
            requirementPrefabList[requirementPrefabList.Count - 1].GetComponent<CraftingRequirementPrefab>().requirements = item.craftingRequirements[i];

            for (int j = 0; j < SO_itemList.Count; j++)
            {
                if (SO_itemList[j].itemName == item.craftingRequirements[i].itemName)
                {
                    requirementPrefabList[requirementPrefabList.Count - 1].GetComponent<CraftingRequirementPrefab>().craftingItemSprite = SO_itemList[j].itemSprite;

                    break;
                }
            }

            requirementPrefabList[requirementPrefabList.Count - 1].GetComponent<CraftingRequirementPrefab>().CheckRequrement();
            requirementPrefabList[requirementPrefabList.Count - 1].GetComponent<CraftingRequirementPrefab>().SetDisplay();

            //Adjust Frame
            craftingScreen.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 67);
        }
    }
    private void CheckForRequiermentsMet()
    {
        int count = 0;

        for (int i = 0; i < requirementPrefabList.Count; i++)
        {
            if (requirementPrefabList[i].GetComponent<CraftingRequirementPrefab>().requirementIsMet)
            {
                count++;
            }
        }

        if (count >= requirementGridLayoutGroup.transform.childCount)
        {
            totalRequirementMet = true;
        }
        else
        {
            totalRequirementMet = false;
        }
    }


    //--------------------


    ItemCategory FindActiveCategoryType()
    {
        for (int i = 0; i < itemCategory_SO.ItemCategoryList.Count; i++)
        {
            if (activeCategory == itemCategory_SO.ItemCategoryList[i].categoryName)
            {
                return itemCategory_SO.ItemCategoryList[i];
            }
        }
        return null;
    }


    //--------------------


    void ActivateItem(Item item)
    {
        for (int i = 0; i < InventorySystem.instance.SO_Item.itemList.Count; i++)
        {
            if (InventorySystem.instance.SO_Item.itemList[i].itemName == item.itemName)
            {
                InventorySystem.instance.SO_Item.itemList[i].isActive = true;
            }
        }
    }
    bool IsItemActive(Item item)
    {
        for (int i = 0; i < InventorySystem.instance.SO_Item.itemList.Count; i++)
        {
            if (InventorySystem.instance.SO_Item.itemList[i].itemName == item.itemName
                && InventorySystem.instance.SO_Item.itemList[i].isActive)
            {
                return true;
            }
        }

        return false;
    }


    //--------------------


    private void OpenInventoryScreen()
    {
        if (craftingScreen_isOpen)
        {
            CloseInventoryScreen();
        }
        else
        {
            SetupItemCategoryList();
            UpdateSelectionSubActiveList();

            craftingMenu.SetActive(true);
            overviewScreen.SetActive(true);
            selectionScreen.SetActive(true);

            craftingScreen_isOpen = true;

            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void CloseInventoryScreen()
    {
        craftingMenu.SetActive(false);
        overviewScreen.SetActive(false);
        selectionScreen.SetActive(false);

        craftingScreen_isOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
    }


    //--------------------



}
