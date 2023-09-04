using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    //Singleton
    public static MainManager instance { get; set; } //Singleton

    public GameObject player;
    public MenuStates menuStates;


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
