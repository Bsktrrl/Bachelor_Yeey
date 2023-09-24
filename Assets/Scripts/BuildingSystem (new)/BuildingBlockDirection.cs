using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockDirection : MonoBehaviour
{
    public GameObject parentBlock;

    public BlockDirection blockDirection;

}

public enum BlockDirection
{
    None,

    North,
    East,
    South,
    West,

    up,
    right,
    down,
    left
}
