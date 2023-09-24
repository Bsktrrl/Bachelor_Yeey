using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockDirection : MonoBehaviour
{
    public GameObject parentBlock;

    public BlockCompass blockDirection;
    public BlockDirection BlockDirection;

}

public enum BlockCompass
{
    None,

    North,
    East,
    South,
    West,

    Cross_A,
    Cross_B
}

public enum BlockDirection
{
    None,

    up,
    right,
    down,
    left
}
