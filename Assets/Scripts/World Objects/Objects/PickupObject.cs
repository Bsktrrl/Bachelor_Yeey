using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Header("item Stats")]
    public bool isActive = true;

    public Items itemName;
    public int amount;

    public bool playerInRange;

    SphereCollider accessCollider = new SphereCollider();

    //For data storage
    public int id;



    //--------------------


    private void Awake()
    {
        //Give all new PickupObjects a unique ID
        //for (int i = 0; i < PickUpObjectsManager.instance.pickupsParents.Count; i++)
        //{
        //    for (int j = 0; j < PickUpObjectsManager.instance.pickupsParents[i].transform.childCount; j++)
        //    {
        //        if (PickUpObjectsManager.instance.pickupsParents[i].transform.GetChild(j).GetComponent<PickupObject>() == this)
        //        {
        //            id = j;
        //        }
        //    }
        //}
    }
    private void Start()
    {
        PlayerButtonManager.leftMouse_isPressedDown += ObjectInteraction;

        //Add SphereCollider for picking up the item
        accessCollider = gameObject.AddComponent<SphereCollider>();
        accessCollider.radius = WorldObjectManager.instance.objectColliderRadius;
        accessCollider.isTrigger = true;
    }


    //--------------------


    public Items GetItemName()
    {
        return itemName;
    }


    //--------------------


    void ObjectInteraction()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject
            && MainManager.instance.menuStates == MenuStates.None)
        {
            EquipmentManager.instance.itemIsbeingEquipped = true;

            StorageManager.instance.AddItem(itemName, amount);
            //InventorySystem.instance.PickUpItem();

            //Remove Subscription to Event
            PlayerButtonManager.leftMouse_isPressedDown -= ObjectInteraction;

            //Update information
            PickUpObjectsManager.instance.UpdatePickupObject_CheckList(itemName, id);

            HandManager.instance.UpdateSlotInfo();

            Destroy(gameObject);
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
