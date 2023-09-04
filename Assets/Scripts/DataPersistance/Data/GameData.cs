using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Variables to Save/Load

    //Player Pos and Rotation
    [HideInInspector] public Vector3 playerPos_Save = new Vector3();
    [HideInInspector] public Quaternion playerRot_Save = new Quaternion();

    //WorldObjects
    public List<ObjectClassSavingVariables> worldObjects_SaveList = new List<ObjectClassSavingVariables>();
    public List<PickupObjectData> pickupObject_CheckSaveList = new List<PickupObjectData>();

    //Inventories
    public List<Inventories> inventories_SaveList = new List<Inventories>();


    //--------------------


    public GameData()
    {
        //Input All Lists to clear
        this.inventories_SaveList.Clear();
        this.worldObjects_SaveList.Clear();
        this.pickupObject_CheckSaveList.Clear();
    }
}
