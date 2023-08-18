using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Header("item Stats")]
    [SerializeField] Items itemName;
    [SerializeField] int amount;

    [HideInInspector] public bool playerInRange;

    SphereCollider accessCollider = new SphereCollider();


    //--------------------


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
            InventorySystem.instance.AddItem(itemName, amount);

            //Remove Subscription to Event
            PlayerButtonManager.leftMouse_isPressedDown -= ObjectInteraction;

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
