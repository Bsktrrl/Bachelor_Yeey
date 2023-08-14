using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonManager : MonoBehaviour
{
    public static Action mouse0_isPressedDown;
    public static Action E_isPressedDown;       //Inventory Screen
    public static Action Esc_isPressedDown;
    public static Action C_isPressedDown;       //Crafting Screen

    public static Action inventory_RightMouse_isPressedDown;
    public static Action inventory_CTRL_and_RightMouse_isPressedDown;
    public static Action inventory_ScrollMouse_isPressedDown;
    public static Action inventory_ScrollMouse_isRolledUP;
    public static Action inventory_ScrollMouse_isRolledDown;


    //--------------------


    private void Update()
    {
        //General Buttons
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouse0_isPressedDown?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            E_isPressedDown?.Invoke();
        }       //Open Inventory
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc_isPressedDown?.Invoke();
        }  //Close Menus   
        if (Input.GetKeyDown(KeyCode.C))
        {
            C_isPressedDown?.Invoke();
        }

        //Inventory Buttons
        if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKey(KeyCode.LeftControl))
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventory_RightMouse_isPressedDown?.Invoke();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftControl))
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventory_CTRL_and_RightMouse_isPressedDown?.Invoke();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
            {
                inventory_ScrollMouse_isPressedDown?.Invoke();
            }
        }
        if (Input.GetKey(KeyCode.Mouse1) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            inventory_ScrollMouse_isRolledUP?.Invoke();
        }
        if (Input.GetKey(KeyCode.Mouse1) && MainManager.instance.menuStates == MenuStates.InventoryMenu && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            inventory_ScrollMouse_isRolledDown?.Invoke();
        }
    }
}
