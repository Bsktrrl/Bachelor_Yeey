using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridItemSlot : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        print("Item is dropped");

        GridInventoryManager.instance.RemoveItemFromInventory(0, gameObject.GetComponent<InteractableObject>().itemName);
    }
}
