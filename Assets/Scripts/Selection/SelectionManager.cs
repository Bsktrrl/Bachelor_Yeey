using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    //Singleton
    public static SelectionManager instance { get; set; }

    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;

    public bool onTarget = false;

    public GameObject selecedObject;


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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            //When reycasting something that is an InteractableObject
            InteractableObject interacteable = selectionTransform.GetComponent<InteractableObject>();

            if (interacteable && interacteable.playerInRange)
            {
                interaction_text.text = interacteable.GetItemName().ToString();
                interaction_Info_UI.SetActive(true);
                onTarget = true;

                selecedObject = interacteable.gameObject;
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

    //--------------------


}