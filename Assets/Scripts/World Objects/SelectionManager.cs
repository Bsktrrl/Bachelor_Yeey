using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
    }

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
                PickupObject pickupObject = selectionTransform.GetComponent<PickupObject>();
                InventoryObject inventoryObject = selectionTransform.GetComponent<InventoryObject>();
                ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();

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

                if (pickupObject && pickupObject.playerInRange)
                {
                    interaction_text.text = pickupObject.GetItemName().ToString();
                    interaction_Info_UI.SetActive(true);
                    onTarget = true;

                    selecedObject = pickupObject.gameObject;
                }
                else if (inventoryObject && inventoryObject.playerInRange)
                {
                    //Show Inventory info
                    onTarget = true;
                    selecedObject = inventoryObject.gameObject;
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

    //--------------------


}