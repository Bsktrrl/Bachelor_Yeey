using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("item Stats")]
    public bool playerInRange;

    public Items itemName;
    public Vector2 itemPos;

    public GridInventoryItem item = new GridInventoryItem();

    SphereCollider accessCollider = new SphereCollider();


    //--------------------


    private void Start()
    {
        PlayerButtonManager.leftMouse_isPressedDown += ObjectInteraction;

        //Add SphereCollider for the item
        accessCollider = gameObject.AddComponent<SphereCollider>();
        accessCollider.radius = WorldObjectManager.instance.objectColliderRadius;
        accessCollider.isTrigger = true;

        //Get correct gridInventoryItem
        for (int i = 0; i < GridInventoryManager.instance.item_SO.itemList.Count; i++)
        {
            if (GridInventoryManager.instance.item_SO.itemList[i].itemName == itemName)
            {
                item.itemName = GridInventoryManager.instance.item_SO.itemList[i].itemName;
                item.itemSize = GridInventoryManager.instance.item_SO.itemList[i].itemSize;

                break;
            }
        }
    }


    //--------------------


    void ObjectInteraction()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject
            && MainManager.instance.menuStates == MenuStates.None)
        {
            //Always add an item from the world to the player's inventory
            if (GridInventoryManager.instance.AddItemToInventory(0, gameObject))
            {
                //Remove this gameObject from the worldObjectList


                //Unsubscribe from Event
                PlayerButtonManager.leftMouse_isPressedDown -= ObjectInteraction;

                //Destroy this gameObject from the world
                Destroy(gameObject);
            }
            else
            {
                //Display message that the inventory cannot take the item because of pace issues

                //Leave this gameObject in the world
            }
        }
    }


    //--------------------


    private void OnTriggerEnter(Collider collision)
    {
        //If a player is entering the area
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        //If a player is exiting the area
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
