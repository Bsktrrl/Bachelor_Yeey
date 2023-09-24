using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock_Parent : MonoBehaviour
{
    public BuildingType buildingType;
    public BlockDirection directionPlaced;
    public GameObject BuildingBlcok;

    public List<GameObject> directionObjects = new List<GameObject>();

    public List<GameObject> ghostList = new List<GameObject>();
}
