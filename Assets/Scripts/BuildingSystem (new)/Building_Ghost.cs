using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Ghost : MonoBehaviour
{
    public BuildingType buildingType;
    public BuildingMaterial buildingMaterial;

    public BlockCompass blockDirection_A;
    public BlockDirection blockDirection_B;

    public bool isSelected;
    //public bool isDeleted;

    public GameObject blockParent;

    
}
