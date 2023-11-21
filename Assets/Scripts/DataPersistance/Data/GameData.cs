using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Player Pos and Rotation
    [HideInInspector] public Vector3 playerPos_Save = new Vector3();
    [HideInInspector] public Quaternion playerRot_Save = new Quaternion();

    //WorldObjects

    //Inventories
    public List<Inventory> Inventories_SaveList = new List<Inventory>();

    //Hotbar
    [HideInInspector] public int selectedSlot_Save = new int();
    public List<Items> hotbarItem_SaveList = new List<Items>();

    //BuidingSystem
    [HideInInspector] public BuildingType buildingType_Save = new BuildingType();
    [HideInInspector] public BuildingMaterial buildingMaterial_Save = new BuildingMaterial();


    //--------------------


    public GameData()
    {
        //Input All Lists to clear
        this.Inventories_SaveList.Clear();
        this.hotbarItem_SaveList.Clear();
    }
}
