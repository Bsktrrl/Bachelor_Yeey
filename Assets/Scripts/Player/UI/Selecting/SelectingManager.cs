using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingManager : MonoBehaviour
{
    public static SelectingManager instance { get; set; } //Singleton

    [SerializeField] GameObject selectingScreen;
    List<Item> SO_item = new List<Item>();

    //public List<GameObject> selectingSlotList = new List<GameObject>();
    //public List<InventoryItem> selectingItemList = new List<InventoryItem>();

    [Header("Other")]
    public bool SelectingScreen_isOpen = true;


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
        //SO_item = InventorySystem.instance.SO_Item.itemList;

        //for (int i = 0; i < selectingSlotList.Count; i++)
        //{
        //    selectingSlotList[i].GetComponentInChildren<DragDrop>().itemImage.sprite = SO_item[0].itemSprite;
        //    selectingSlotList[i].GetComponentInChildren<DragDrop>().amountText.text = "";
        //}
    }


    //--------------------


    public void OpenCloseSelectingScreen()
    {
        if (SelectingScreen_isOpen)
        {
            selectingScreen.SetActive(false);
            SelectingScreen_isOpen = false;
        }
        else
        {
            selectingScreen.SetActive(true);
            SelectingScreen_isOpen = true;
        }
    }


    //--------------------


}
