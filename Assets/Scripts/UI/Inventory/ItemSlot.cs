using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            //if there are any item on this GameObject
            if (transform.childCount > 0)
            {
                //If the item type is the same, add it to the amount

                //If the stack are full, don't do anything

                return transform.GetChild(0).gameObject;
            }

            return null;
        }
    }


    //--------------------


    //If something is dropped on this GameObject
    public void OnDrop(PointerEventData eventData)
    {
        //if there is not an item on this GameObject
        if (!Item)
        {
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
        }
    }
}