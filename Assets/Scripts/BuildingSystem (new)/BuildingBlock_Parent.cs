using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock_Parent : MonoBehaviour
{
    public BuildingType buildingType;

    public BlockCompass directionPlaced_A;
    public BlockDirection directionPlaced_B;

    public GameObject BuildingBlcok;

    public List<GameObject> directionObjects = new List<GameObject>();

    public List<GameObject> ghostList = new List<GameObject>();
}
