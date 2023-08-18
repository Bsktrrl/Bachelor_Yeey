using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public class DataManager : MonoBehaviour, IDataPersistance
{
    public static DataManager instance { get; private set; } //Singleton
    public static Action dataIsSaving;
    public static Action datahasLoaded;

    //Variables to Save/Load
    [HideInInspector] public Vector3 playerPos_Store = new Vector3();
    [HideInInspector] public Quaternion playerRot_Store = new Quaternion();

    [HideInInspector] public List<ObjectClassSavingVariables> worldObjects_StoreList = new List<ObjectClassSavingVariables>();
    [HideInInspector] public List<Inventories> inventories_StoreList = new List<Inventories>();


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
        //Input what to store
        this.playerPos_Store = gameData.playerPos_Save;
        this.playerRot_Store = gameData.playerRot_Save;
        this.inventories_StoreList = gameData.inventories_SaveList;
        this.worldObjects_StoreList = gameData.worldObjects_SaveList;

        datahasLoaded?.Invoke();
    }

    public void SaveData(ref GameData gameData)
    {
        dataIsSaving?.Invoke();

        //Input what to save
        gameData.playerPos_Save = this.playerPos_Store;
        gameData.playerRot_Save = this.playerRot_Store;
        gameData.inventories_SaveList = this.inventories_StoreList;
        gameData.worldObjects_SaveList = this.worldObjects_StoreList;
    }
}