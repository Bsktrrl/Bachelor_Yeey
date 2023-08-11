using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] [TextArea (5, 10)] string ItemName;

    public bool playerInRange;


    //--------------------

    private void Start()
    {
        PlayerButtonManager.mouse0_isPressedDown += PickUpItem;
    }


    //--------------------


    public string GetItemName()
    {
        return ItemName;
    }


    //--------------------


    void PickUpItem()
    {
        if (playerInRange && SelectionManager.instance.onTarget)
        {
            //Remove Subscription to Event
            PlayerButtonManager.mouse0_isPressedDown -= PickUpItem;

            //Add item to inventory

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