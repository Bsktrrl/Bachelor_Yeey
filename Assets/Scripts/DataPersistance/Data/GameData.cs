using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<GameObject> inventoryObject = new List<GameObject>();
    public List<Inventories> inventories = new List<Inventories>();


    //--------------------


    public GameData()
    {
        this.inventories.Clear();
    }
}
