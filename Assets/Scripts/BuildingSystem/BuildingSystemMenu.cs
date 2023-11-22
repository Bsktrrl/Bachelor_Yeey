using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSystemMenu : MonoBehaviour
{
    public static BuildingSystemMenu instance { get; set; } //Singleton

    public GameObject buildingSystemMenu;
    public Image selectedBuildingBlockImage;

    public List<GameObject> buildingBlockUIList = new List<GameObject>();

    public bool buildingSystemMenu_isOpen;


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
        PlayerButtonManager.isPressed_BuildingSystemMenu_Enter += BuildingBlockSelecter_Enter;
        PlayerButtonManager.isPressed_BuildingSystemMenu_Exit += BuildingBlockSelecter_Exit;

        buildingSystemMenu.SetActive(false);
    }


    //--------------------


    public void SetSelectedImage(Sprite sprite)
    {
        selectedBuildingBlockImage.sprite = sprite;
    }


    //--------------------


    void BuildingBlockSelecter_Enter()
    {
        buildingSystemMenu_isOpen = true;

        Cursor.lockState = CursorLockMode.None;
        MainManager.instance.menuStates = MenuStates.BuildingSystemMenu;
        BuildingManager.instance.SetAllGhostState_Off();

        buildingSystemMenu.SetActive(true);
    }
    void BuildingBlockSelecter_Exit()
    {
        buildingSystemMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        MainManager.instance.menuStates = MenuStates.None;

        buildingSystemMenu_isOpen = false;
    }
}
