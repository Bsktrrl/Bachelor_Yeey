using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("item Stats")]
    [SerializeField] Items itemName;
    [SerializeField] int amount;

    [HideInInspector] public bool playerInRange;

    SphereCollider accessCollider = new SphereCollider();


    //--------------------


    private void Awake()
    {
        //Add SphereCollider for picking up the item
        accessCollider = gameObject.AddComponent<SphereCollider>();
        accessCollider.radius = 5;
        accessCollider.isTrigger = true;
    }
    private void Start()
    {
        PlayerButtonManager.mouse0_isPressedDown += PickUpItem;
    }


    //--------------------


    public Items GetItemName()
    {
        return itemName;
    }


    //--------------------


    void PickUpItem()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject)
        {
            if (InventorySystem.instance.AddItemToInventory(itemName, amount))
            {
                print("Successfully added Items");
            }
            else
            {
                print("The inventory is Full");

                //Spawn a box in the world with this item and its amount to be picked up later
            }

            //Remove Subscription to Event
            PlayerButtonManager.mouse0_isPressedDown -= PickUpItem;

            Destroy(gameObject);

            ////If there are room in the inventory
            //if (!InventorySystem.instance.CheckIfFull())
            //{
            //    

            //    //Add item to inventory
            //    InventorySystem.instance.AddItemToInventory(itemName, amount);

            //    Destroy(gameObject);
            //}
            //else
            //{

            //}
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