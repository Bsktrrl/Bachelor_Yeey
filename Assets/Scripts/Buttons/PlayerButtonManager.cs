using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerButtonManager : MonoBehaviour
{
    //Singleton
    public static PlayerButtonManager instance { get; set; } //Singleton

    public InventoryButtonState inventoryButtonState = InventoryButtonState.None;

    public static Action leftMouse_isPressedDown;
    public static Action rightMouse_isPressedDown;
    public static Action Tab_isPressedDown;       //Inventory Screen
    public static Action Esc_isPressedDown;
    public static Action E_isPressedDown;
    public static Action C_isPressedDown;       //Crafting Screen

    public static Action inventory_RightMouse_isPressedDown;
    public static Action inventory_Shift_and_RightMouse_isPressedDown;
    public static Action inventory_ScrollMouse_isPressedDown;
    public static Action inventory_ScrollMouse_isRolledUP;
    public static Action inventory_ScrollMouse_isRolledDown;

    //Testing
    public static Action S_isClicked;
    public static Action A_isClicked;
    public static Action R_isClicked;
    public static Action O_isClicked;


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
    private void Update()
    {
        //General Buttons
        if ((Input.GetKeyDown(KeyCode.Mouse0) && inventoryButtonState == InventoryButtonState.None)
            || (Input.GetKeyDown(KeyCode.Mouse0) && inventoryButtonState == InventoryButtonState.mouse0_isPressedDown))
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.mouse0_isPressedDown;
            }

            leftMouse_isPressedDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            rightMouse_isPressedDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Tab_isPressedDown?.Invoke();
        }       //Open Inventory
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc_isPressedDown?.Invoke();
        }  //Close Menus   
        else if (Input.GetKeyDown(KeyCode.C))
        {
            C_isPressedDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            E_isPressedDown?.Invoke();
        }

        //Inventory Buttons
        else if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKeyDown(KeyCode.Mouse1) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.inventory_RightMouse_isPressedDown;
                inventory_RightMouse_isPressedDown?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKey(KeyCode.LeftShift) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.inventory_RightMouse_isPressedDown;
                inventory_RightMouse_isPressedDown?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftShift) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.inventory_Shift_and_RightMouse_isPressedDown;
                inventory_Shift_and_RightMouse_isPressedDown?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.inventory_ScrollMouse_isPressedDown;
                inventory_ScrollMouse_isPressedDown?.Invoke();
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            inventory_ScrollMouse_isRolledUP?.Invoke();
        }
        else if (Input.GetKey(KeyCode.Mouse1) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            inventory_ScrollMouse_isRolledDown?.Invoke();
        }

        //Testing
        else if (Input.GetKeyDown(KeyCode.S))
        {
            S_isClicked?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            A_isClicked?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            R_isClicked?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            O_isClicked?.Invoke();
        }
    }
}

public enum InventoryButtonState
{
    None,

    mouse0_isPressedDown,
    inventory_RightMouse_isPressedDown,
    inventory_Shift_and_RightMouse_isPressedDown,
    inventory_ScrollMouse_isPressedDown
}
