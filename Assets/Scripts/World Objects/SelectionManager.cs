using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; set; } //Singleton

    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;

    public bool onTarget = false;

    public GameObject selecedObject;
    public GameObject selectedTree;

    public GameObject chopHolder;


    //--------------------


    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (MainManager.instance.menuStates == MenuStates.None)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var selectionTransform = hit.transform;

                //When reycasting something that is interactable
                NewInteractableObject newInteractableObject = selectionTransform.GetComponent<NewInteractableObject>();
                ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();

                //PickupObject pickupObject = selectionTransform.GetComponent<PickupObject>();
                //InteractableObject interactableObject = selectionTransform.GetComponent<InteractableObject>();
                //InventoryObject inventoryObject = selectionTransform.GetComponent<InventoryObject>();

                //If looking at a tree
                if (choppableTree && choppableTree.playerInRange)
                {
                    choppableTree.canBeChopped = true;
                    selectedTree = choppableTree.gameObject;
                    chopHolder.gameObject.SetActive(true);
                }
                else
                {
                    if (selectedTree != null)
                    {
                        selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                        selectedTree = null;
                    }

                    chopHolder.gameObject.SetActive(false);
                }

                //if (pickupObject && pickupObject.playerInRange)
                //{
                //    interaction_text.text = pickupObject.GetItemName().ToString();
                //    interaction_Info_UI.SetActive(true);
                //    onTarget = true;

                //    selecedObject = pickupObject.gameObject;
                //}
                //else if (interactableObject && interactableObject.playerInRange)
                //{
                //    interaction_text.text = interactableObject.itemName.ToString();
                //    interaction_Info_UI.SetActive(true);
                //    onTarget = true;

                //    selecedObject = interactableObject.gameObject;
                //}
                //else if (inventoryObject && inventoryObject.playerInRange)
                //{
                //    //Show Inventory info
                //    onTarget = true;
                //    selecedObject = inventoryObject.gameObject;
                //}

                //If looking at an Ineracteable Object
                if (newInteractableObject && newInteractableObject.playerInRange)
                {
                    //Show Inventory info
                    onTarget = true;
                    selecedObject = newInteractableObject.gameObject;
                }

                //If there is a Hit without an interacteable script
                else
                {
                    interaction_Info_UI.SetActive(false);
                    onTarget = false;
                }
            }
            //If there is no script attached at all
            else
            {
                interaction_Info_UI.SetActive(false);
                onTarget = false;
            }
        }
    }

}