using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
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


    //--------------------


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        dragging_Parent = InventorySystem.instance.inventoryDraggingParent;
    }


    //--------------------


    public void OnBeginDrag(PointerEventData eventData)
    {
        for (int i = 0; i < InventorySystem.instance.inventorySlotList.Count; i++)
        {
            if (InventorySystem.instance.inventorySlotList[i].GetComponent<ItemSlot>().gameObject.GetComponentInChildren<DragDrop>() == this)
            {
                print("OnBeginDrag");

                InventorySystem.instance.activeInventorySlotList_Index = i;

                break;
            }
        }

        canvasGroup.alpha = .6f;

        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
        startParent = transform.parent;

        //transform.SetParent(transform.root);
        transform.parent = dragging_Parent.transform;
        transform.SetAsLastSibling();
        itemBeingDragged = gameObject;

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
    }
}