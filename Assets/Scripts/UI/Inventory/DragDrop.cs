using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
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

    


    //--------------------


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        dragging_Parent = InventorySystem.instance.inventoryDraggingParent;
    }
    private void Start()
    {
        selectedFrameImage.gameObject.SetActive(false);
    }


    //--------------------


    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySystem.instance.itemIsDragging = true;

        //Get index to the original field
        for (int i = 0; i < InventorySystem.instance.inventorySlotList.Count; i++)
        {
            if (InventorySystem.instance.inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>() == this)
            {
                print("OnBeginDrag");

                InventorySystem.instance.activeInventorySlotList_Index = i;

                break;
            }
        }

        canvasGroup.alpha = .75f;

        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
        startParent = transform.parent;

        transform.position = eventData.position;
        transform.parent = dragging_Parent.transform;
        transform.SetAsLastSibling();
        itemBeingDragged = gameObject;

        EnterDisplayItem();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed) and so it will be consistant if the canvas has a different scale (other then 1);
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;

        transform.SetParent(startParent);

        if (transform.parent == startParent || transform.parent == dragging_Parent.transform)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        InventorySystem.instance.itemIsDragging = false;
    }


    //--------------------


    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterDisplayItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitDisplayItem();
    }


    //--------------------

    
    void EnterDisplayItem()
    {
        selectedFrameImage.gameObject.SetActive(true);

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
                            print("Hover Item");

                            InventorySystem.instance.selecteditemImage.sprite = SO_Item.itemList[j].itemSprite;
                            InventorySystem.instance.selecteditemName.text = SO_Item.itemList[j].itemName.ToString();
                            InventorySystem.instance.selecteditemDescription.text = SO_Item.itemList[j].itemDescription;

                            break;
                        }
                        else
                        {
                            print("Hover None");

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
}