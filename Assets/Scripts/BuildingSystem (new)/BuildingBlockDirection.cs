using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockDirection : MonoBehaviour
{
    public GameObject parentBlock;

    public BlockCompass blockDirection_A;
    public BlockDirection blockDirection_B;
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

    Up,
    Right,
    Down,
    Left
}
