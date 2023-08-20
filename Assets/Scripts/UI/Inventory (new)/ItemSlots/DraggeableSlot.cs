using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggeableSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    public Transform startParent;
    public GameObject dragging_Parent;
    public static GameObject itemBeingDragged;
    Vector2 startPosition;

    public bool isClicked;
    public bool isDragged;

    [SerializeField] PointerEventData buttonPressed;

    [Header("Parent")]
    public GameObject parentObject;
    public ItemSlot_N parentScript;

    [Header("Display")]
    public Image itemImage;
    public TextMeshProUGUI itemAmountText;
    public GameObject selectedFrame;

    [Header("Ghost")]
    public GameObject ghostObject;
    public Image ghostImage;
    public TextMeshProUGUI ghostAmountText;


    //--------------------


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (startParent == null)
        {
            dragging_Parent = InventorySystem.instance.inventoryDraggingParent;
        }

        selectedFrame.SetActive(false);
        ghostObject.SetActive(false);
        ghostImage.gameObject.SetActive(false);
        ghostAmountText.gameObject.SetActive(false);
    }
    private void Start()
    {
        startParent = transform.parent;

        UpdateInventoryDisplay();
    }
    private void Update()
    {
        if(!parentScript.onDrop)
        {
            UpdateInventoryDisplay();
        }
    }


    //--------------------


    public void UpdateInventoryDisplay()
    {
        //Update Display
        if (parentScript.itemInThisSlot.itemName != Items.None)
        {
            for (int i = 0; i < StorageManager.instance.item_SO.itemList.Count; i++)
            {
                if (StorageManager.instance.item_SO.itemList[i].itemName == parentScript.itemInThisSlot.itemName)
                {
                    itemImage.sprite = StorageManager.instance.item_SO.itemList[i].itemSprite;
                    ghostImage.sprite = StorageManager.instance.item_SO.itemList[i].itemSprite;

                    itemAmountText.text = parentScript.itemInThisSlot.amount.ToString();
                    ghostAmountText.text = StorageManager.instance.itemAmountLeftBehind.ToString();

                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < StorageManager.instance.item_SO.itemList.Count; i++)
            {
                if (StorageManager.instance.item_SO.itemList[i].itemName == parentScript.itemInThisSlot.itemName)
                {
                    itemImage.sprite = StorageManager.instance.item_SO.itemList[0].itemSprite;
                    ghostImage.sprite = StorageManager.instance.item_SO.itemList[0].itemSprite;

                    itemAmountText.text = "";
                    ghostAmountText.text = "";

                    break;
                }
            }
        }

        //Update InventoryManager.instance.inventories
        InventoryManager.instance.inventories[parentScript.itemInThisSlotFromInventory].itemList[parentScript.slotIndexInInventory] = parentScript.itemInThisSlot;
    }


    //--------------------


    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = eventData;

        if (!StorageManager.instance.itemIsClicked)
        {
            StorageManager.instance.itemIsClicked = true;
            isClicked = true;

            //Get index to the original activeInventorySlotList_Index
            bool activeItemFound = false;
            if (!activeItemFound)
            {
                for (int i = 0; i < StorageManager.instance.PlayerInventoryItemSlotList.Count; i++)
                {
                    if (StorageManager.instance.PlayerInventoryItemSlotList[i].GetComponent<ItemSlot_N>().draggeableSlotScript == this)
                    {
                        StorageManager.instance.activeSlotList = StorageManager.instance.PlayerInventoryItemSlotList;
                        StorageManager.instance.activeSlotList_Index = i;
                        StorageManager.instance.activeInventoryItem = parentScript.itemInThisSlot;
                        StorageManager.instance.activeInventoryList_Index = parentScript.itemInThisSlotFromInventory;

                        activeItemFound = true;

                        break;
                    }
                }
            }
            if (!activeItemFound)
            {
                for (int i = 0; i < StorageManager.instance.StorageBoxItemSlotList.Count; i++)
                {
                    if (StorageManager.instance.StorageBoxItemSlotList[i].GetComponent<ItemSlot_N>().draggeableSlotScript == this)
                    {
                        StorageManager.instance.activeSlotList = StorageManager.instance.StorageBoxItemSlotList;
                        StorageManager.instance.activeSlotList_Index = i;
                        StorageManager.instance.activeInventoryItem = parentScript.itemInThisSlot;
                        StorageManager.instance.activeInventoryList_Index = parentScript.itemInThisSlotFromInventory;

                        activeItemFound = true;

                        break;
                    }
                }
            }

            if (StorageManager.instance.activeInventoryItem.itemName != Items.None)
            {
                StorageManager.instance.selectedItemIsEmpty = false;

                ghostImage.gameObject.SetActive(true);
                ghostObject.SetActive(true);
            }
            else
            {
                StorageManager.instance.selectedItemIsEmpty = true;

                ghostImage.gameObject.SetActive(false);
                ghostObject.SetActive(false);
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!parentScript.onDrop)
        {
            StorageManager.instance.activeInventoryItem.amount = StorageManager.instance.maxItemInSelectedStack;
        }

        StorageManager.instance.itemIsClicked = false;
        isClicked = false;

        if (transform.parent == startParent)
        {
            UpdateInventoryDisplay();
        }

        ghostImage.gameObject.SetActive(false);
        ghostObject.SetActive(false);

        PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySystem.instance.itemIsDragging = true;

        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
        startParent = transform.parent;

        //transform.position = eventData.position;
        transform.SetParent(dragging_Parent.transform);
        transform.SetAsLastSibling();
        itemBeingDragged = gameObject;
        isDragged = true;

        EnterDisplayItem();

        //ghostAmountText.text = StorageManager.instance.itemAmountLeftBehind.ToString();
        ghostAmountText.gameObject.SetActive(true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed) and so it will be consistant if the canvas has a different scale (other then 1);
        rectTransform.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        isDragged = false;

        transform.SetParent(startParent);

        if (transform.parent == startParent || transform.parent == dragging_Parent.transform)
        {
            UpdateInventoryDisplay();

            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        canvasGroup.blocksRaycasts = true;

        ghostAmountText.gameObject.SetActive(false);

        StorageManager.instance.itemIsDragging = false;
        StorageManager.instance.itemIsSplitted = false;

        buttonPressed = null;

        PlayerButtonManager.instance.inventoryButtonState = InventoryButtonState.None;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySelect_Clip();
        EnterDisplayItem();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ExitisplayItem();
    }


    //--------------------


    void EnterDisplayItem()
    {
        //Set SelectedFrame
        if (!isDragged)
        {
            selectedFrame.SetActive(true);
        }
        else
        {
            if (!StorageManager.instance.selectedItemIsEmpty)
            {
                selectedFrame.SetActive(false);
            }
        }

        //Setup PlayerInventoryDisplay
        List<Item> itemList = StorageManager.instance.item_SO.itemList;
        if (!StorageManager.instance.itemIsDragging)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (parentScript.itemInThisSlot.itemName == itemList[0].itemName)
                {
                    StorageManager.instance.itemSprite_Display.sprite = itemList[0].itemSprite;
                    StorageManager.instance.itemName_Display.text = "";
                    StorageManager.instance.itemDescription_Display.text = "";

                    break;
                }
                else if (parentScript.itemInThisSlot.itemName == itemList[i].itemName)
                {
                    StorageManager.instance.itemSprite_Display.sprite = itemList[i].itemSprite;
                    StorageManager.instance.itemName_Display.text = itemList[i].itemName.ToString();
                    StorageManager.instance.itemDescription_Display.text = itemList[i].itemDescription;

                    break;
                }
            }
        }
    }
    void ExitisplayItem()
    {
        //Set PlayerInventoryDisplay to Empty
        selectedFrame.SetActive(false);

        if (!StorageManager.instance.itemIsDragging)
        {
            List<Item> itemList = StorageManager.instance.item_SO.itemList;

            StorageManager.instance.itemSprite_Display.sprite = itemList[0].itemSprite;
            StorageManager.instance.itemName_Display.text = "";
            StorageManager.instance.itemDescription_Display.text = "";
        }
    }
}
