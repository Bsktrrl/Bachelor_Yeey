using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSystemMenu : MonoBehaviour
{
    public static BuildingSystemMenu instance { get; set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
    }

    [SerializeField] GameObject buildingSystemMenu;
    public Image selectedBuildingBlockImage;


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

        buildingSystemMenu.SetActive(false);
    }
    private void Start()
    {
        PlayerButtonManager.isPressed_BuildingSystemMenu_Enter += BuildingBlockSelecter_Enter;
        PlayerButtonManager.isPressed_BuildingSystemMenu_Exit += BuildingBlockSelecter_Exit;
    }


    //--------------------


    public void SetSelectedImage(Sprite sprite)
    {
        selectedBuildingBlockImage.sprite = sprite;
    }


    //--------------------


    void BuildingBlockSelecter_Enter()
    {
        print("Pressing Mouse - Enter");
        buildingSystemMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        MainManager.instance.menuStates = MenuStates.BuildingSystemMenu;

        BuildingManager.instance.SetAllGhostState_Off();
    }
    void BuildingBlockSelecter_Exit()
    {
        print("Pressing Mouse - Exit");
        buildingSystemMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        MainManager.instance.menuStates = MenuStates.None;
    }
}
