using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewInteractableObject : MonoBehaviour
{
    public bool playerInRange;

    public Items itemName;
    [SerializeField] InteracteableType interacteableType;
    [SerializeField] int inventoryIndex;

    SphereCollider accessCollider = new SphereCollider();


    //--------------------


    private void Start()
    {
        PlayerButtonManager.E_isPressedDown += ObjectInteraction;

        //Add SphereCollider for the item
        accessCollider = gameObject.AddComponent<SphereCollider>();
        Vector3 scale = gameObject.transform.lossyScale;
        accessCollider.radius = WorldObjectManager.instance.objectColliderRadius / scale.x / 2; //Chenge to only "objectColliderRadius"
        accessCollider.isTrigger = true;
    }


    //--------------------


    void ObjectInteraction()
    {
        if (playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selecedObject == gameObject
            && MainManager.instance.menuStates == MenuStates.None)
        {
            print("Interract with Pickup");

            //If Object is a Pickup
            if (interacteableType == InteracteableType.Pickup)
            {
                //Check If item can be added
                if (NewGridInventoryManager.instance.AddItemToInventory(0, gameObject, false))
                {
                    //Unsubscribe from Event
                    PlayerButtonManager.E_isPressedDown -= ObjectInteraction;

                    //Destroy gameObject
                    Destroy(gameObject);
                }
            }

            //If Object is an Inventory
            else if (interacteableType == InteracteableType.Inventory)
            {
                print("Interract with Inventory");

                //Open the player Inventory
                NewGridInventoryManager.instance.OpenPlayerInventory();

                //Open the chest Inventory
                NewGridInventoryManager.instance.chestInventoryOpen = inventoryIndex;
                NewGridInventoryManager.instance.PrepareInventoryUI(inventoryIndex, false); //Prepare Player Inventory
                NewGridInventoryManager.instance.chestInventory_Parent.GetComponent<RectTransform>().sizeDelta = NewGridInventoryManager.instance.inventories[inventoryIndex].inventorySize * NewGridInventoryManager.instance.cellsize;
                NewGridInventoryManager.instance.chestInventory_Parent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(NewGridInventoryManager.instance.cellsize, NewGridInventoryManager.instance.cellsize);
                NewGridInventoryManager.instance.chestInventory_Parent.SetActive(true);
            }

            //If Object is a machine
            else if (interacteableType == InteracteableType.Machine)
            {
                print("Interract with Machine");

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

public enum InteracteableType
{
    None,

    Pickup,
    Inventory,
    Machine
}