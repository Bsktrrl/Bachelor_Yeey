using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHammer : MonoBehaviour
{
    public void PlaceBlock()
    {
        BuildingManager.instance.PlaceBlock();
    }
}
