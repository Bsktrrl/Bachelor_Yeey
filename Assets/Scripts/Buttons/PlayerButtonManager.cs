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


    //--------------------


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouse0_isPressedDown?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            E_isPressedDown?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc_isPressedDown?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            C_isPressedDown?.Invoke();
        }
    }
}
