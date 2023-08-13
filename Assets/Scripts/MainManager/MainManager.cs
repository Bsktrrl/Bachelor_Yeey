using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    //Singleton
    public static MainManager instance { get; set; } //Singleton

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


    //--------------------


}

public enum MenuStates
{
    None,
    MainMenu,
    PauseMenu,
    InventoryMenu
}
