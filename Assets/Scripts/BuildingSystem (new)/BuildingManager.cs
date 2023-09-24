using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance { get; set; } //Singleton
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Reinitialize()
    {
        instance = null; 
    }

    public GameObject buildingBlock_Parent;
    public List<GameObject> buildingBlockList = new List<GameObject>();

    [Header("Ghost")]
    public GameObject lastBuildingBlock_LookedAt;
    //public GameObject lastMarker_LookedAt;
    //public GameObject buildingBlock_PointedAt;
    public GameObject ghost_PointedAt;
    public bool buildingBlockCanBePlaced;
    public string BlockTagName;

    [Header("Selected")]
    [SerializeField] Vector2 BuildingDistance;
    public BlockCompass buildingBlockDirection_Selected;
    public BuildingType buildingType_Selected = BuildingType.None;
    public BuildingMaterial buildingMaterial_Selected = BuildingMaterial.None;

    [Header("BuildingBlocks List")]
    [SerializeField] GameObject builingBlock_Floor;
    [SerializeField] GameObject builingBlock_Floor_Triangle;
    [SerializeField] GameObject builingBlock_Wall;
    [SerializeField] GameObject builingBlock_Wall_Diagonal;
    [SerializeField] GameObject builingBlock_Angle;

    [Header("Materials List")]
    [SerializeField] Material invisible_Material;
    [SerializeField] Material ghost_Material;
    [SerializeField] Material canPlace_Material;
    [SerializeField] Material cannotPlace_Material;

    //Wood
    [SerializeField] Material floor_Wood_Material;
    [SerializeField] Material wall_Wood_Material;
    [SerializeField] Material wall_Diagonaly_Wood_Material;


    //--------------------


    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        //Only active when not in a menu
        if (MainManager.instance.menuStates == MenuStates.None)
        {
            RaycastBuidingDirectionMarkers();
        }
    }


    //--------------------


    void RaycastBuidingDirectionMarkers()
    {
        if (HandManager.instance.selectedSlotItem.subCategoryName == ItemSubCategories.BuildingHammer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Get the Transform of GameObject hit
                var hitTransform = hit.transform;

                BlockTagName = hitTransform.tag;

                if ((hitTransform.gameObject.CompareTag("BuidingDirectionMarkers")
                    || hitTransform.gameObject.CompareTag("BuildingBlock_Ghost")
                    || hitTransform.gameObject.CompareTag("BuildingBlock"))
                    && hit.distance > BuildingDistance.x)
                {
                    SetAllGhostState_Off();
                    return;
                }
                else if ((hitTransform.gameObject.CompareTag("BuidingDirectionMarkers")
                    || hitTransform.gameObject.CompareTag("BuildingBlock_Ghost")
                    || hitTransform.gameObject.CompareTag("BuildingBlock"))
                    && hit.distance < BuildingDistance.y)
                {
                    SetAllGhostState_Off();
                    return;
                }

                //Get the BuildingBlockDirection
                if (hitTransform.gameObject.CompareTag("BuidingDirectionMarkers") || hitTransform.gameObject.CompareTag("BuildingBlock_Ghost"))
                {
                    if (hitTransform.gameObject.CompareTag("BuidingDirectionMarkers"))
                    {
                        switch (hitTransform.gameObject.GetComponent<BuildingBlockDirection>().blockDirection)
                        {
                            case BlockCompass.None:
                                break;

                            case BlockCompass.North:
                                if (buildingBlockDirection_Selected != BlockCompass.North)
                                {
                                    buildingBlockDirection_Selected = BlockCompass.North;
                                }
                                break;
                            case BlockCompass.East:
                                if (buildingBlockDirection_Selected != BlockCompass.East)
                                {
                                    buildingBlockDirection_Selected = BlockCompass.East;
                                }
                                break;
                            case BlockCompass.South:
                                if (buildingBlockDirection_Selected != BlockCompass.South)
                                {
                                    buildingBlockDirection_Selected = BlockCompass.South;
                                }
                                break;
                            case BlockCompass.West:
                                if (buildingBlockDirection_Selected != BlockCompass.West)
                                {
                                    buildingBlockDirection_Selected = BlockCompass.West;
                                }
                                break;
                            case BlockCompass.Cross_A:
                                if (buildingBlockDirection_Selected != BlockCompass.Cross_A)
                                {
                                    buildingBlockDirection_Selected = BlockCompass.Cross_A;
                                }
                                break;
                            case BlockCompass.Cross_B:
                                if (buildingBlockDirection_Selected != BlockCompass.Cross_B)
                                {
                                    buildingBlockDirection_Selected = BlockCompass.Cross_B;
                                }
                                break;

                            default:
                                break;
                        }

                        FindGhostDirection(hitTransform.gameObject.GetComponent<BuildingBlockDirection>().parentBlock.GetComponent<BuildingBlock_Parent>());
                    }

                    if (hitTransform.gameObject.CompareTag("BuildingBlock"))
                    {
                        lastBuildingBlock_LookedAt = hitTransform.gameObject.GetComponent<BuildingBlock>().buidingBlock_Parent;
                    }
                    else if (hitTransform.gameObject.CompareTag("BuidingDirectionMarkers"))
                    {
                        lastBuildingBlock_LookedAt = hitTransform.gameObject.GetComponent<BuildingBlockDirection>().parentBlock;
                    }
                    else if (hitTransform.gameObject.CompareTag("BuildingBlock_Ghost"))
                    {
                        lastBuildingBlock_LookedAt = hitTransform.gameObject.GetComponent<Building_Ghost>().blockParent;
                    }
                }
                else
                {
                    if (hitTransform.gameObject.CompareTag("BuildingBlock"))
                    {
                        lastBuildingBlock_LookedAt = hitTransform.gameObject.GetComponent<BuildingBlock>().buidingBlock_Parent;
                    }
                    else if (hitTransform.gameObject.CompareTag("BuidingDirectionMarkers"))
                    {
                        lastBuildingBlock_LookedAt = hitTransform.gameObject.GetComponent<BuildingBlockDirection>().parentBlock;
                    }
                    else if (hitTransform.gameObject.CompareTag("BuildingBlock_Ghost"))
                    {
                        lastBuildingBlock_LookedAt = hitTransform.gameObject.GetComponent<Building_Ghost>().blockParent;
                    }

                    //If raycarsting is not on a buildingBlock or ghostBlock
                    if (buildingBlockDirection_Selected != BlockCompass.None)
                    {
                        buildingBlockDirection_Selected = BlockCompass.None;
                        SetAllGhostState_Off();

                        if (ghost_PointedAt != null)
                        {
                            ghost_PointedAt.SetActive(false);
                            ghost_PointedAt = null;
                        }
                    }
                }
            }
            else
            {
                //When raycast doesn't hit any BuildingObjects
                if (buildingBlockDirection_Selected != BlockCompass.None)
                {
                    buildingBlockDirection_Selected = BlockCompass.None;
                    SetAllGhostState_Off();

                    if (ghost_PointedAt != null)
                    {
                        ghost_PointedAt.SetActive(false);
                        ghost_PointedAt = null;
                    }
                }
            }
        }
        else
        {
            //When Hammer isn't in the hand anymore
            if (buildingBlockDirection_Selected != BlockCompass.None)
            {
                buildingBlockDirection_Selected = BlockCompass.None;
                SetAllGhostState_Off();

                if (ghost_PointedAt != null)
                {
                    ghost_PointedAt.SetActive(false);
                    ghost_PointedAt = null;
                }
            }
        }
    }
    void FindGhostDirection(BuildingBlock_Parent buildingBlock)
    {
        switch (buildingBlockDirection_Selected)
        {
            case BlockCompass.None:
                break;

            case BlockCompass.North:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockCompass.North)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockCompass.East:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockCompass.East)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockCompass.South:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockCompass.South)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockCompass.West:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockCompass.West)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockCompass.Cross_A:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockCompass.Cross_A)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockCompass.Cross_B:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockCompass.Cross_B)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;

            default:
                break;
        }
    }

    void SetGhostState_ON(BuildingBlock_Parent buildingBlock, int i)
    {
        //Floor
        if (buildingType_Selected == BuildingType.Floor)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(buildingBlock, i, BuildingType.Floor, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Floor - triangle
        else if (buildingType_Selected == BuildingType.Triangle)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(buildingBlock, i, BuildingType.Triangle, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Wall
        else if (buildingType_Selected == BuildingType.Wall)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(buildingBlock, i, BuildingType.Wall, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //WallDiagonal
        else if (buildingType_Selected == BuildingType.Wall_Diagonaly)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(buildingBlock, i, BuildingType.Wall_Diagonaly, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Angled
        else if (buildingType_Selected == BuildingType.Angeled)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(buildingBlock, i, BuildingType.Angeled, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Turn off
        else
        {
            SetGhostState_OFF(buildingBlock, i);
        }
    }
    void SetGhostState_OFF(BuildingBlock_Parent buildingBlock, int i)
    {
        buildingBlock.ghostList[i].SetActive(false);
        buildingBlock.ghostList[i].GetComponent<MeshRenderer>().material = invisible_Material;
        buildingBlock.ghostList[i].GetComponent<Building_Ghost>().isSelected = false;
    }
    void SetAllGhostState_Off()
    {
        for (int i = 0; i < buildingBlockList.Count; i++)
        {
            for (int j = 0; j < buildingBlockList[i].GetComponent<BuildingBlock_Parent>().ghostList.Count; j++)
            {
                SetGhostState_OFF(buildingBlockList[i].GetComponent<BuildingBlock_Parent>(), j);
            }
        }

        buildingBlockCanBePlaced = false;
    }

    void BuidingBlockCanBePlacedCheck(BuildingBlock_Parent buildingBlock, int i, BuildingType buildingType, Material material_Can, Material material_Cannot)
    {
        if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().buildingType != buildingType)
        {
            return;
        }

        //Reset all ghost before setting a new one
        if (ghost_PointedAt != buildingBlock.ghostList[i])
        {
            SetAllGhostState_Off();
            ghost_PointedAt = buildingBlock.ghostList[i];
        }

        //Can be placed
        //print("Overlapping Ghosts: " + CheckOverlappingGhost());
        if (!CheckOverlappingGhost()
            && buildingBlock.ghostList[i].GetComponent<Building_Ghost>().buildingType == buildingType)
        {
            //Insert functionality to prevent a selected ghost to be placed, and change its color to red

            //if()...
            //buildingBlock.ghostList[i].GetComponent<MeshRenderer>().material = cannotPlace_Material;
            //buildingBlock.ghostList[i].GetComponent<Building_Ghost>().isSelected = true;
            //buildingBlock.ghostList[i].SetActive(true);
            //buildingBlockCanBePlaced = false;

            buildingBlock.ghostList[i].GetComponent<MeshRenderer>().material = material_Can;
            buildingBlock.ghostList[i].GetComponent<Building_Ghost>().isSelected = true;
            buildingBlock.ghostList[i].SetActive(true);
            buildingBlockCanBePlaced = true;
        }

        //Cannot be placed
        else
        {
            buildingBlockCanBePlaced = false;
            SetGhostState_OFF(buildingBlock, i);
        }
    }
    bool CheckOverlappingGhost()
    {
        //Floor
        if (buildingType_Selected == BuildingType.Floor)
        {
            //Check if ghost_PointedAt has the same position as any other buildingblock
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                if (ghost_PointedAt.transform.position == buildingBlockList[i].transform.position)
                {
                    SetAllGhostState_Off();
                    return true;
                }
            }
        }

        //Floor - Triangle
        else if (buildingType_Selected == BuildingType.Triangle)
        {
            //Check if ghost_PointedAt has the same position as any other buildingblock
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                if (ghost_PointedAt.transform.position == buildingBlockList[i].transform.position)
                {
                    SetAllGhostState_Off();
                    return true;
                }
            }
        }

        //Wall
        else if (buildingType_Selected == BuildingType.Wall)
        {
            //Check if ghost_PointedAt has the same position and rotation as any other buildingblock
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                //Spacial treatment for the South, since the position is the same any time
                //if (buildingBlockDirection_Selected == BlockDirection.South)
                //{
                //    if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().transform.position
                //        == ghost_PointedAt.transform.position
                //        && buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                //        && ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall)
                //    {
                //        SetAllGhostState_Off();
                //        return true;
                //    }

                //}
                if (buildingBlockDirection_Selected == BlockCompass.South)
                {
                    Vector3 tempSouth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    Vector3 tempNorth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);

                    if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.South)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempSouth)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                    else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.North)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempNorth)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                }
                else if (buildingBlockDirection_Selected == BlockCompass.North)
                {
                    Vector3 tempSouth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);
                    Vector3 tempNorth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);

                    if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.South)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempSouth)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                    else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.North)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempNorth)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                }
                
                else if (buildingBlockDirection_Selected == BlockCompass.West)
                {
                    Vector3 tempEast = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    Vector3 tempWest = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);

                    if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.East)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempEast)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                    else if(buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.West)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempWest)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                }
                else if (buildingBlockDirection_Selected == BlockCompass.East)
                {
                    Vector3 tempEast = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    Vector3 tempWest = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);

                    if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.East)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempEast)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                    else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced == BlockCompass.West)
                        && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempWest)
                    {
                        SetAllGhostState_Off();
                        return true;
                    }
                }
            }
        }

        //Wall - Diagonally
        else if (buildingType_Selected == BuildingType.Wall_Diagonaly)
        {
            //Check if ghost_PointedAt has the same position as any other buildingblock
            //for (int i = 0; i < buildingBlockList.Count; i++)
            //{
            //    if (ghost_PointedAt.transform.position == buildingBlockList[i].transform.position)
            //    {
            //        SetAllGhostState_Off();
            //        return true;
            //    }
            //}
        }

        //Angeled
        else if (buildingType_Selected == BuildingType.Angeled)
        {
            //Check if ghost_PointedAt has the same position as any other buildingblock
            //for (int i = 0; i < buildingBlockList.Count; i++)
            //{
            //    if (ghost_PointedAt.transform.position == buildingBlockList[i].transform.position)
            //    {
            //        SetAllGhostState_Off();
            //        return true;
            //    }
            //}
        }

        return false;
    }


    //--------------------


    public void PlaceBlock()
    {
        if (ghost_PointedAt != null && buildingBlockCanBePlaced)
        {
            //Floor
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Floor
                && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Floor");

                    SetAllGhostState_Off();
                    SoundManager.instance.PlayWood_Placed_Clip();

                    buildingBlockList.Add(Instantiate(builingBlock_Floor) as GameObject);
                    buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                    buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;
                    buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = buildingBlockDirection_Selected;
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    //print("Placed: Stone");

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.iron)
                {
                    //print("Placed: Iron");

                }
            }

            //Floor - triangle
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Triangle
                && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Triangle");
                    
                    SetAllGhostState_Off();
                    SoundManager.instance.PlayWood_Placed_Clip();

                    if (buildingBlockDirection_Selected == BlockCompass.South)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Floor_Triangle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;
                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.South;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.East)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Floor_Triangle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y -90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.North)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Floor_Triangle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y - 180,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.West)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Floor_Triangle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y + 90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    //print("Placed: Stone");

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.iron)
                {
                    //print("Placed: Iron");

                }
            }

            //Wall
            else if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall
                && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Wall");

                    SetAllGhostState_Off();
                    SoundManager.instance.PlayWood_Placed_Clip();

                    buildingBlockList.Add(Instantiate(builingBlock_Wall) as GameObject);
                    buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                    buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                    //Change Rotation
                    if (buildingBlockDirection_Selected == BlockCompass.North)
                    {
                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y + 180,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.North;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.East)
                    {
                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y - 90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.South)
                    {
                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.South;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.West)
                    {
                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y + 90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.West;
                    }
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    //print("Placed: Stone");

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.iron)
                {
                    //print("Placed: Iron");

                }
            }


            //Wall_Diagonal
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall_Diagonaly
                && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Wall_Diagonaly");

                    SetAllGhostState_Off();
                    SoundManager.instance.PlayWood_Placed_Clip();

                    if (buildingBlockDirection_Selected == BlockCompass.Cross_A)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Wall_Diagonal) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;
                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.South;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.Cross_B)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Wall_Diagonal) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y - 90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    //print("Placed: Stone");

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.iron)
                {
                    //print("Placed: Iron");

                }
            }

            //Angle
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Angeled
                && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Angeled");

                    SetAllGhostState_Off();
                    SoundManager.instance.PlayWood_Placed_Clip();

                    if (buildingBlockDirection_Selected == BlockCompass.South)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Angle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;
                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.South;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.East)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Angle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y - 90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.North)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Angle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y - 180,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                    else if (buildingBlockDirection_Selected == BlockCompass.West)
                    {
                        buildingBlockList.Add(Instantiate(builingBlock_Angle) as GameObject);
                        buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                        buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                        buildingBlockList[buildingBlockList.Count - 1].transform.Rotate
                        (
                            ghost_PointedAt.transform.rotation.x,
                            ghost_PointedAt.transform.rotation.y + 90,
                            ghost_PointedAt.transform.rotation.z,
                            Space.World
                        );

                        buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced = BlockCompass.East;
                    }
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    //print("Placed: Stone");

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.iron)
                {
                    //print("Placed: Iron");

                }
            }
        }

        lastBuildingBlock_LookedAt = null;
        buildingBlockCanBePlaced = false;
        SetAllGhostState_Off();
    }
}
