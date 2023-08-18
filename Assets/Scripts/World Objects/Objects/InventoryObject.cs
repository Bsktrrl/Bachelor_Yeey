using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class InventoryObject : MonoBehaviour
{
    public bool playerInRange;

    [Header("Category")]
    public ObjectType objectType;

    [Header("Indexes")]
    public int objectIndex;
    public int inventoryIndex;

    SphereCollider accessCollider = new SphereCollider();


    //--------------------


    private void Start()
    {
        PlayerButtonManager.E_isPressedDown += ObjectInteraction;
        PlayerButtonManager.rightMouse_isPressedDown += DeleteThisObject;

        //Add SphereCollider for picking up the item
        accessCollider = gameObject.AddComponent<SphereCollider>();
        accessCollider.radius = WorldObjectManager.instance.objectColliderRadius;
        accessCollider.isTrigger = true;
    }


    //--------------------


    void ObjectInteraction()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject
            && MainManager.instance.menuStates == MenuStates.None)
        {
            //What happen when interacting with inventory

            print("Interacting with Chest: " + objectIndex);
        }
    }
    void DeleteThisObject()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject
            && MainManager.instance.menuStates == MenuStates.None)
        {
            WorldObjectManager.instance.DeleteObjectFromTheWorld(objectType, objectIndex, inventoryIndex);

            Destroy(gameObject);

            print("Deleting Chest: " + objectIndex);

            PlayerButtonManager.E_isPressedDown -= ObjectInteraction;
            PlayerButtonManager.rightMouse_isPressedDown -= DeleteThisObject;
        }
    }
    public void SetIndex(int i)
    {
        objectIndex = i;
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