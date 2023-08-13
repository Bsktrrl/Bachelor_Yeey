using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    Vector2 startPosition;
    Transform startParent;
    GameObject dragging_Parent;


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

        if (transform.parent == startParent || transform.parent == dragging_Parent.transform)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}