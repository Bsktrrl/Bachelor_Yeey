using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Ghost : MonoBehaviour
{
    public BuildingType buildingType;
    public BuildingMaterial buildingMaterial;

    public BlockDirection blockDirection;

    public bool isSelected;
    public bool isDeleted;

    public GameObject blockParent;
}

public enum BlockDirection
{
    None,

    North,
    East,
    South,
    West
}
