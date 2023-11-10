using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridItemSlot : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        InteractableObject item = gameObject.GetComponent<InteractableObject>();

        //If itemName == None, find the item that is overlapping with this
        if (item.itemName == Items.None)
        {
            ////Search through all itemSlots in the grid
            //for (int i = 0; i < GridInventoryManager.instance.gridItemsList.Count; i++)
            //{
            //    //If an itemSlot contain an item, seach if this.pos == any of the items pos in the grid
            //    if (GridInventoryManager.instance.gridItemsList[i].item.itemName != Items.None)
            //    {
            //        Vector2 pos = GridInventoryManager.instance.gridItemsList[i].item.itemSize;

            //        //Search thorugh all pos of the item
            //        for (int x = 0; x < pos.x; x++)
            //        {
            //            for (int y = 0; y < pos.y; y++)
            //            {
            //                if (item.itemPos == new Vector2((GridInventoryManager.instance.gridItemsList[i].x * 100) + x, (GridInventoryManager.instance.gridItemsList[i].y * 100) - y))
            //                {
            //                    GridInventoryManager.instance.RemoveItemFromInventory(item.item.isInInventory, GridInventoryManager.instance.gridItemsList[i].item.itemName);

            //                    print("Item is dropped, the advanced way");
            //                    return;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        //If itemName != None, remove it as normal
        else
        {
            GridInventoryManager.instance.RemoveItemFromInventory(item.item.isInInventory, item.itemName);
            print("Item is dropped, the normal way");
        }
    }
}
