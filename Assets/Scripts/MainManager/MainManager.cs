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
