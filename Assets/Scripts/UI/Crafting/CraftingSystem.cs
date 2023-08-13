using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    //Singleton
    public static CraftingSystem instance { get; set; }

    public GameObject craftingScreenUI;
    public GameObject toolScreenUI;

    public List<string> inventoryItemList = new List<string>();

    //Category Buttons
    [SerializeField] Button toolsButton;


    public bool isOpen;

    //AllBlueprints



    //--------------------


    private void Awake()
    {
        //Singleton
        if (instance != null & instance != this)
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
        isOpen = false;

        PlayerButtonManager.C_isPressedDown += OpenInventoryScreen;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;

        craftingScreenUI.SetActive(false);
        toolScreenUI.SetActive(false);
    }


    //--------------------


    public void ScreenTransition()
    {
        toolsButton.gameObject.GetComponent<ScreenTransition>().MoveToNewScreen();
    }


    //--------------------


    void OpenInventoryScreen()
    {
        if (!isOpen)
        {
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            isOpen = true;
        }
        else if (isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolScreenUI.SetActive(false);

            if (!InventorySystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }
    }
    void CloseInventoryScreen()
    {
        if (isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolScreenUI.SetActive(false);

            if (!InventorySystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }
    }
}
