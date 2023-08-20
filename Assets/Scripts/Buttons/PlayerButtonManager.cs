using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerButtonManager : MonoBehaviour
{
    //Singleton
    public static PlayerButtonManager instance { get; set; } //Singleton

    public ButtonClickedState buttonClickedState = ButtonClickedState.None;
    public InventoryButtonState inventoryButtonState = InventoryButtonState.None;

    public static Action leftMouse_isPressedDown;
    public static Action rightMouse_isPressedDown;
    public static Action Tab_isPressedDown;       //Inventory Screen
    public static Action Esc_isPressedDown;
    public static Action E_isPressedDown;

    public static Action inventory_LeftMouse_isPressedDown;
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
        //Left Mouse
        if (Input.GetKeyDown(KeyCode.Mouse0) && inventoryButtonState == InventoryButtonState.None && buttonClickedState == ButtonClickedState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.leftMouse;
                inventory_LeftMouse_isPressedDown?.Invoke();
            }
            else
            {
                buttonClickedState = ButtonClickedState.leftMouse;
                leftMouse_isPressedDown?.Invoke();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.None;
            }
            else
            {
                buttonClickedState = ButtonClickedState.None;
            }
        }

        //Right Mouse
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse1) && inventoryButtonState == InventoryButtonState.None && buttonClickedState == ButtonClickedState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.rightMouse;
                inventory_RightMouse_isPressedDown?.Invoke();
            }
            else
            {
                buttonClickedState = ButtonClickedState.rightMouse;
                rightMouse_isPressedDown?.Invoke();
            }

        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.None;
            }
            else
            {
                buttonClickedState = ButtonClickedState.None;
            }
        }
        
        //Menus
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            buttonClickedState = ButtonClickedState.tab;
            Tab_isPressedDown?.Invoke();
        }       //Open Inventory
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            buttonClickedState = ButtonClickedState.Esc;
            Esc_isPressedDown?.Invoke();
        }  //Close Menus   
        
        //Object Interraction
        else if (Input.GetKeyDown(KeyCode.E))
        {
            buttonClickedState = ButtonClickedState.E;
            E_isPressedDown?.Invoke();
        }

        //Inventory Buttons
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse1) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.Shift_RightMouse;
                inventory_Shift_and_RightMouse_isPressedDown?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventoryButtonState = InventoryButtonState.ScrollWheel;
                inventory_ScrollMouse_isPressedDown?.Invoke();
            }
        }
        else if ((Input.GetKey(KeyCode.Mouse1) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") > 0)
            || (Input.GetKey(KeyCode.Mouse0) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") > 0))
        {
            inventory_ScrollMouse_isRolledUP?.Invoke();
        }
        else if ((Input.GetKey(KeyCode.Mouse1) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") < 0)
            || (Input.GetKey(KeyCode.Mouse0) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") < 0))
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
        
        else
        {
            buttonClickedState = ButtonClickedState.None;
        }
    }
}
public enum ButtonClickedState
{
    None,

    leftMouse,
    rightMouse,
    middleMouse,

    tab,
    shift,
    Esc,
    C,
    E
}
public enum InventoryButtonState
{
    None,

    leftMouse,
    rightMouse,
    Shift_RightMouse,
    ScrollWheel
}

