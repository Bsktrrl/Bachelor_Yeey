using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    None,

    Floor,
    Triangle,
    Wall,
    Wall_Diagonaly,
    Ramp,
    Ramp_Corner
}
public enum BuildingSubType
{
    None,

    Wall_Diagonaly
}

public enum BuildingMaterial
{
    None,

    Wood,
    Stone,
    Iron
}