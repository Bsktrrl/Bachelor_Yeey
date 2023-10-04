using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Ghost : MonoBehaviour
{
    [Header("Type")]
    public BuildingType buildingType;
    public BuildingSubType buildingSubType;

    [Header("Material")]
    public BuildingMaterial buildingMaterial;

    [Header("Directions")]
    public BlockCompass blockDirection_A;
    public BlockDirection blockDirection_B;

    [Header("Other")]
    public GameObject blockParent;
    public bool isSelected;
}
