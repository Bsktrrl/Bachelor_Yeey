using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager instance { get; set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
    }

    public GameObject player;
    public MenuStates menuStates;
    public GameStates gameStates;

    [Header("Parents")]
    public GameObject treeParent;


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
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        UpdateGameStates();
    }


    //--------------------


    void UpdateGameStates()
    {
        //Set to Building
        if (HandManager.instance.selectedSlotItem.subCategoryName == ItemSubCategories.BuildingHammer)
        {
            gameStates = GameStates.Building;
        }

        //Set to None
        else
        {
            gameStates = GameStates.None;
        }
    }


    //--------------------


    void Save()
    {
        DataPersistanceManager.instance.SaveGame();
    }
}

public enum MenuStates
{
    None,

    MainMenu,
    PauseMenu,
    InventoryMenu
}

public enum GameStates
{
    None,

    Building
}