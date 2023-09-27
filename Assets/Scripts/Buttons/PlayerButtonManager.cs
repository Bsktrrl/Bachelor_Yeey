using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerButtonManager : MonoBehaviour
{
    //Singleton
    public static PlayerButtonManager instance { get; set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null;
        leftMouse_isPressedDown = null;
        rightMouse_isPressedDown = null;
        Tab_isPressedDown = null;
        Esc_isPressedDown = null;
        E_isPressedDown = null;

        //Inventory
        inventory_LeftMouse_isPressedDown = null;
        inventory_RightMouse_isPressedDown = null;
        inventory_Shift_and_RightMouse_isPressedDown = null;
        inventory_ScrollMouse_isPressedDown = null;
        inventory_ScrollMouse_isRolledUP = null;
        inventory_ScrollMouse_isRolledDown = null;
        moveStackToStorageBox = null;

        handSelection_Down = null;
        handSelection_Up = null;

        //HandSelected
        isPressed_1 = null;
        isPressed_2 = null;
        isPressed_3 = null;
        isPressed_4 = null;
        isPressed_5 = null;
        isPressed_6 = null;
        isPressed_7 = null;
        isPressed_8 = null;
        isPressed_9 = null;

        //BuildingSystem
        isPressed_R_Rotate = null;

        //Testing Buttons
        T_isPressed = null;
}

    public ButtonClickedState buttonClickedState = ButtonClickedState.None;
    public InventoryButtonState inventoryButtonState = InventoryButtonState.None;

    public static Action leftMouse_isPressedDown;
    public static Action rightMouse_isPressedDown;
    public static Action Tab_isPressedDown;       //Inventory Screen
    public static Action Esc_isPressedDown;
    public static Action E_isPressedDown;

    //Inventory
    public static Action inventory_LeftMouse_isPressedDown;
    public static Action inventory_RightMouse_isPressedDown;
    public static Action inventory_Shift_and_RightMouse_isPressedDown;
    public static Action inventory_ScrollMouse_isPressedDown;
    public static Action inventory_ScrollMouse_isRolledUP;
    public static Action inventory_ScrollMouse_isRolledDown;
    public static Action moveStackToStorageBox;

    public static Action handSelection_Down;
    public static Action handSelection_Up;

    //HandSelected
    public static Action isPressed_1;
    public static Action isPressed_2;
    public static Action isPressed_3;
    public static Action isPressed_4;
    public static Action isPressed_5;
    public static Action isPressed_6;
    public static Action isPressed_7;
    public static Action isPressed_8;
    public static Action isPressed_9;

    //BuildingSystem
    public static Action isPressed_R_Rotate;

    //Testing Buttons
    public static Action T_isPressed;



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
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0) && inventoryButtonState == InventoryButtonState.None && buttonClickedState == ButtonClickedState.None)
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
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0) && inventoryButtonState == InventoryButtonState.None)
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                moveStackToStorageBox?.Invoke();
            }
        }
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

        //Select Hand
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            handSelection_Down?.Invoke();
        }
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            handSelection_Up?.Invoke();
        }

        //Hand QuickAction
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha1))
            isPressed_1?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha2))
            isPressed_2?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha3))
            isPressed_3?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha4))
            isPressed_4?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha5))
            isPressed_5?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha6))
            isPressed_6?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha7))
            isPressed_7?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha8))
            isPressed_8?.Invoke();
        else if (MainManager.instance.menuStates == MenuStates.None && Input.GetKey(KeyCode.Alpha9))
            isPressed_9?.Invoke();

        //BuildingSystem
        else if (Input.GetKeyDown(KeyCode.R) && MainManager.instance.gameStates == GameStates.Building && MainManager.instance.menuStates == MenuStates.None)
        {
            isPressed_R_Rotate?.Invoke();
        }


        //Testing
        else if (Input.GetKeyDown(KeyCode.T))
        {
            T_isPressed?.Invoke();
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
    ScrollWheel,
    QuickClick
}

