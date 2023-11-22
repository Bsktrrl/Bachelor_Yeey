using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock_Parent : MonoBehaviour
{
    public BuildingType buildingType;
    public BuildingSubType buildingSubType;
    public BuildingMaterial buildingMaterial;
    public bool isStrange;

    //public BlockCompass directionPlaced_A;
    //public BlockDirection directionPlaced_B;

    public GameObject BuildingBlock;

    public List<GameObject> directionObjects = new List<GameObject>();

    public List<BlockPlaced> blockPlacedList = new List<BlockPlaced>();

    public List<GameObject> ghostList = new List<GameObject>();
    
    private void Start()
    {
        //Let the player don't collide with all directionObjects
        for (int i = 0; i < directionObjects.Count; i++)
        {
            Physics.IgnoreCollision(MainManager.instance.player.GetComponent<CharacterController>(), directionObjects[i].GetComponent<Collider>(), true);
        }

        //Let the player don't collide with all ghostListObjects
        for (int i = 0; i < ghostList.Count; i++)
        {
            Physics.IgnoreCollision(MainManager.instance.player.GetComponent<CharacterController>(), ghostList[i].GetComponent<Collider>(), true);
        }
    }

}

[Serializable]
public class BlockPlaced
{
    public GameObject buildingBlock;
    public BuildingType buildingType;
    public BuildingSubType buildingSubType;
    public bool isStrange;

    //public BlockCompass directionPlaced_A;
    //public BlockDirection directionPlaced_B;
}
