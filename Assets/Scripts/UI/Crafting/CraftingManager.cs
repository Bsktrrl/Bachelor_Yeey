using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance { get; set; } //Singleton

    [Header("Main Screen")]
    [SerializeField] GameObject craftingMenu;

    [Header("Overview Screen")]
    [SerializeField] ItemCategory_SO itemCategory_SO;
    [SerializeField] GameObject overviewScreen;
    [SerializeField] GameObject overviewGridLayoutGroop;
    [SerializeField] GameObject categoryButton_Prefab;
    public List<ItemCategory> categoryItems = new List<ItemCategory>();
    public List<GameObject> categoryButtonPrefabList = new List<GameObject>();

    [Header("Selection Screen")]
    [SerializeField] GameObject selectionScreen;
    [SerializeField] Image categorySelectedImage;
    [SerializeField] TextMeshProUGUI categorySelectedName;

    [Header("Crafting Screen")]
    [SerializeField] GameObject craftingScreen;
    [SerializeField] GameObject craftingScreen_Prefab;
    [SerializeField] GameObject requirement_Prefab;

    [Header("Other")]
    public bool craftingScreen_isOpen;
    public ItemCategories activeCategory;


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

        craftingMenu.SetActive(false);
        overviewScreen.SetActive(false);
        selectionScreen.SetActive(false);
        craftingScreen.SetActive(false);

        SetupItemCategoryList();

        //Set the first useable Category in the itemCategory_SO.ItemCategoryList to be active by default
        activeCategory = itemCategory_SO.ItemCategoryList[1].categoryName;
        UpdateSelectionScreen();
    }


    //--------------------


    //Overview Screen
    void SetupItemCategoryList()
    {
        //Reset overviewScreen
        overviewScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);

        //Instantiate CategoryButtons
        for (int i = 1; i < itemCategory_SO.ItemCategoryList.Count; i++)
        {
            categoryButtonPrefabList.Add(Instantiate(categoryButton_Prefab) as GameObject);
            categoryButtonPrefabList[categoryButtonPrefabList.Count - 1].transform.SetParent(overviewGridLayoutGroop.transform);
            categoryButtonPrefabList[categoryButtonPrefabList.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = itemCategory_SO.ItemCategoryList[i].categorySprite;

            categoryButtonPrefabList[categoryButtonPrefabList.Count - 1].GetComponent<CategoryButton>().categoryType = itemCategory_SO.ItemCategoryList[i].categoryName;

            //Adjust Frame
            overviewScreen.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 85);
        }
    }


    //--------------------


    //Selection Screen
    public void UpdateSelectionScreen()
    {
        categorySelectedImage.sprite = FindActiveCategoryType().categorySprite;
        categorySelectedName.text = FindActiveCategoryType().categoryName.ToString();
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


    private void OpenInventoryScreen()
    {
        if (craftingScreen_isOpen)
        {
            CloseInventoryScreen();
        }
        else
        {
            craftingMenu.SetActive(true);
            overviewScreen.SetActive(true);
            selectionScreen.SetActive(true);

            craftingScreen_isOpen = true;
        }
    }
    private void CloseInventoryScreen()
    {
        craftingMenu.SetActive(false);
        overviewScreen.SetActive(false);
        selectionScreen.SetActive(false);

        craftingScreen_isOpen = false;
    }


    //--------------------



}
