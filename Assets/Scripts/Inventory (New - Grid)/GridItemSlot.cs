using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridItemSlot : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        InteractableObject thisItem = gameObject.GetComponent<InteractableObject>();

        //If itemName == None, find the item that is overlapping with this
        if (thisItem.itemName == Items.None)
        {
            //Calculate if another item is placed on this.gameObject's coordinate
            List<InteractableObject> inventoryItemsTemp = new List<InteractableObject>();

            for (int i = 0; i < GridInventoryManager.instance.playerInventory_Parent.transform.childCount; i++)
            {
                inventoryItemsTemp.Add(GridInventoryManager.instance.playerInventory_Parent.transform.GetChild(i).gameObject.GetComponent<InteractableObject>());

                if (inventoryItemsTemp[inventoryItemsTemp.Count - 1].itemName == Items.None)
                {
                    print(i + ". Name: -");
                }
                else
                {
                    print(i + ". Name: " + inventoryItemsTemp[inventoryItemsTemp.Count - 1].itemName);
                }
            }

            //Search through all itemSlots in the grid
            for (int i = 0; i < inventoryItemsTemp.Count; i++)
            {
                //If an itemSlot contain an item, search if this.pos == any of the items pos in the grid
                if (inventoryItemsTemp[i].item.itemName != Items.None)
                {
                    Vector2 pos = inventoryItemsTemp[i].item.itemSize;

                    //Search thorugh all pos of the item
                    for (int x = 0; x < pos.x; x++)
                    {
                        for (int y = 0; y < pos.y; y++)
                        {
                            if (thisItem.itemPos == new Vector2(inventoryItemsTemp[i].gameObject.GetComponent<RectTransform>().anchoredPosition.x + (x * GridInventoryManager.instance.cellSize), inventoryItemsTemp[i].gameObject.GetComponent<RectTransform>().anchoredPosition.y - (y * GridInventoryManager.instance.cellSize)))
                            {
                                GridInventoryManager.instance.RemoveItemFromInventory(thisItem.item.isInInventory, inventoryItemsTemp[i].item.itemName);

                                print("Item is dropped, the advanced way");
                                return;
                            }
                        }
                    }
                }
            }
        }

        //If itemName != None, remove it as normal
        else
        {
            GridInventoryManager.instance.RemoveItemFromInventory(thisItem.item.isInInventory, thisItem.itemName);
            //print("Item is dropped, the normal way");

            print("GridItemSlot - Dropped: " + thisItem.itemName);
        }
    }
}
