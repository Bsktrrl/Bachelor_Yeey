using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    Vector2 startPosition;
    public Transform startParent;
    public GameObject dragging_Parent;

    public Vector2 startItemslotPos;
    public Vector2 endItemslotPos;

    public Image itemImage;
    public TextMeshProUGUI amountText;
    public Image selectedFrameImage;

    public bool isDragged;
    public bool isClicked;




    //--------------------


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (startParent == null)
        {
            dragging_Parent = InventorySystem.instance.inventoryDraggingParent;
        }
    }
    private void Start()
    {
        selectedFrameImage.gameObject.SetActive(false);
        startParent = transform.parent;
    }


    //--------------------


    public void OnPointerDown(PointerEventData eventData)
    {
        switch (PlayerButtonManager.instance.inventoryButtonState)
        {
            case InventoryButtonState.None:
                break;
            case InventoryButtonState.mouse0_isPressedDown:
                return;
                break;
            case InventoryButtonState.inventory_RightMouse_isPressedDown:
                return;
                break;
            case InventoryButtonState.inventory_Shift_and_RightMouse_isPressedDown:
                return;
                break;
            case InventoryButtonState.inventory_ScrollMouse_isPressedDown:
                return;
                break;
            default:
                break;
        }

        InventorySystem.instance.itemIsClicked = true;

        //Get index to the original activeInventorySlotList_Index
        for (int i = 0; i < InventorySystem.instance.inventorySlotList.Count; i++)
        {
            if (InventorySystem.instance.inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>() == this)
            {
                InventorySystem.instance.activeInventorySlotList_Index = i;

                break;
            }
        }

        isClicked = true;
        //canvasGroup.alpha = .75f;

        InventorySystem.instance.CreateDragDropTemp(startParent, this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
        //canvasGroup.alpha = 1f;

        if (transform.parent == startParent)
        {
            InventorySystem.instance.UpdateInventoryDisplay();
        }

        InventorySystem.instance.itemIsClicked = false;

        InventorySystem.instance.DeleteDragDropTemp(this);

        //Check for button buggs
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

        if (InventorySystem.instance.dragDropTempList.Count > 0)
        {
            InventorySystem.instance.dragDropTempList[InventorySystem.instance.dragDropTempList.Count - 1].GetComponent<DragDrop>().amountText.gameObject.SetActive(true);
        }
        else
        {
            //Nothing
        }
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
            InventorySystem.instance.UpdateInventoryDisplay();

            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        canvasGroup.blocksRaycasts = true;

        InventorySystem.instance.itemIsDragging = false;
        InventorySystem.instance.itemIsSplitted = false;
    }


    //--------------------


    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySelect_Clip();
        EnterDisplayItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitDisplayItem();
    }


    //--------------------

    
    void EnterDisplayItem()
    {
        if (!isDragged)
        {
            selectedFrameImage.gameObject.SetActive(true);
        }
        else
        {
            if (!InventorySystem.instance.selectedItemIsEmpty)
            {
                selectedFrameImage.gameObject.SetActive(false);
            }
        }

        if (!InventorySystem.instance.itemIsDragging)
        {
            for (int i = 0; i < InventorySystem.instance.inventorySlotList.Count; i++)
            {
                if (InventorySystem.instance.inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>() == this)
                {
                    Item_SO SO_Item = InventorySystem.instance.SO_Item;

                    for (int j = 0; j < SO_Item.itemList.Count; j++)
                    {
                        if (InventorySystem.instance.inventoryItemList[i].itemName == SO_Item.itemList[j].itemName
                            && InventorySystem.instance.inventoryItemList[i].itemName != Items.None)
                        {
                            InventorySystem.instance.selecteditemImage.sprite = SO_Item.itemList[j].itemSprite;
                            InventorySystem.instance.selecteditemName.text = SO_Item.itemList[j].itemName.ToString();
                            InventorySystem.instance.selecteditemDescription.text = SO_Item.itemList[j].itemDescription;

                            break;
                        }
                        else
                        {
                            InventorySystem.instance.selecteditemImage.sprite = SO_Item.itemList[0].itemSprite;
                            InventorySystem.instance.selecteditemName.text = "";
                            InventorySystem.instance.selecteditemDescription.text = "";
                        }
                    }

                    break;
                }
            }
        }
    }
    void ExitDisplayItem()
    {
        selectedFrameImage.gameObject.SetActive(false);

        if (!InventorySystem.instance.itemIsDragging)
        {
            Item_SO SO_Item = InventorySystem.instance.SO_Item;

            InventorySystem.instance.selecteditemImage.sprite = SO_Item.itemList[0].itemSprite;
            InventorySystem.instance.selecteditemName.text = "";
            InventorySystem.instance.selecteditemDescription.text = "";
        }
    }


    //--------------------

    
    public void DeleteThisObject()
    {
        Destroy(gameObject);
    }
}