using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public class DataManager : MonoBehaviour, IDataPersistance
{
    public static DataManager instance { get; set; } //Singleton

    //public bool itemIsActivated = false; //Test


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
        //this.itemIsActivated = gameData.itemIsActivated; //Test
    }

    public void SaveData(ref GameData gameData)
    {
        //gameData.itemIsActivated = this.itemIsActivated; //Test
    }
}