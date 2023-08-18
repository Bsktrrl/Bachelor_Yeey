using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public class DataManager : MonoBehaviour, IDataPersistance
{
    public static DataManager instance { get; private set; } //Singleton
    public static Action datahasLoaded;

    //Variables to Save/Load
    [HideInInspector] public List<GameObject> inventoryObject = new List<GameObject>();
    [HideInInspector] public List<Inventories> inventories = new List<Inventories>();


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


    public void LoadData(GameData gameData)
    {
        this.inventories = gameData.inventories;
        datahasLoaded?.Invoke();
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.inventories = this.inventories;
    }
}