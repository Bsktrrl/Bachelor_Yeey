using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance { get; set; } //Singleton

    [Header("Main Screen")]
    [SerializeField] GameObject craftingMenu;

    [Header("Overview Screen")]
    [SerializeField] GameObject overviewScreen;
    [SerializeField] GameObject overviewGridLayoutGroop;
    [SerializeField] GameObject categoryButton_Prefab;

    [Header("Selection Screen")]
    [SerializeField] GameObject selectionScreen;
    [SerializeField] Image categorySelectedImage;
    [SerializeField] TextMeshProUGUI categorySelectedName;

    [Header("Crafting Screen")]
    [SerializeField] GameObject craftingScreen;
    [SerializeField] GameObject craftingScreen_Prefab;
    [SerializeField] GameObject requirement_Prefab;

    [Header("Other")]
    public bool craftingScreen_isOpen;

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
        PlayerButtonManager.Tab_isPressedDown += OpenInventoryScreen;
        PlayerButtonManager.Esc_isPressedDown += CloseInventoryScreen;

        craftingMenu.SetActive(false);
        overviewScreen.SetActive(false);
        selectionScreen.SetActive(false);
        craftingScreen.SetActive(false);
    }


    //--------------------


    private void OpenInventoryScreen()
    {
        if (craftingScreen_isOpen)
        {
            CloseInventoryScreen();
        }
        else
        {
            craftingMenu.SetActive(true);
            overviewScreen.SetActive(true);
            selectionScreen.SetActive(true);
        }
        
        craftingScreen_isOpen = true;
    }

    private void CloseInventoryScreen()
    {
        craftingMenu.SetActive(false);
        overviewScreen.SetActive(false);
        selectionScreen.SetActive(false);

        craftingScreen_isOpen = false;
    }


    //--------------------



}
