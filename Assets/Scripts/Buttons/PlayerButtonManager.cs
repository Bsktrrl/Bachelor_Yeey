using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonManager : MonoBehaviour
{
    public static Action mouse0_isPressedDown;


    //--------------------


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouse0_isPressedDown?.Invoke();
        }
    }
}
