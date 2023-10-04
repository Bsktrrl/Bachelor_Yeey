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
    public GameObject ghost_PointedAt;
    public bool buildingBlockCanBePlaced;
    public string BlockTagName;
    Ray oldRay = new Ray();

    [Header("Selected")]
    public BuildingType buildingType_Selected = BuildingType.None;

    public BuildingMaterial buildingMaterial_Selected = BuildingMaterial.None;

    [SerializeField] Vector2 BuildingDistance;
    public BlockCompass blockDirection_A;
    public BlockDirection blockDirection_B;

    #region BuildingBlocks List
    [Header("BuildingBlocks List - Wood")]
    [SerializeField] GameObject builingBlock_Wood_Floor;
    [SerializeField] GameObject builingBlock_Wood_Floor_Triangle;
    [SerializeField] GameObject builingBlock_Wood_Wall;
    [SerializeField] GameObject builingBlock_Wood_Wall_Diagonal;
    [SerializeField] GameObject builingBlock_Wood_Angle;

    [Header("BuildingBlocks List - Stone")]
    [SerializeField] GameObject builingBlock_Stone_Floor;
    [SerializeField] GameObject builingBlock_Stone_Floor_Triangle;
    [SerializeField] GameObject builingBlock_Stone_Wall;
    [SerializeField] GameObject builingBlock_Stone_Wall_Diagonal;
    [SerializeField] GameObject builingBlock_Stone_Angle;

    [Header("BuildingBlocks List - Iron")]
    [SerializeField] GameObject builingBlock_Iron_Floor;
    [SerializeField] GameObject builingBlock_Iron_Floor_Triangle;
    [SerializeField] GameObject builingBlock_Iron_Wall;
    [SerializeField] GameObject builingBlock_Iron_Wall_Diagonal;
    [SerializeField] GameObject builingBlock_Iron_Angle;
    #endregion

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
    private void Start()
    {
        PlayerButtonManager.isPressed_R_Rotate += RotateBlock;
    }

    private void Update()
    {
        RaycastSetup();
    }


    //--------------------


    void RaycastSetup()
    {
        //Only active when not in a menu
        if (HandManager.instance.selectedSlotItem.subCategoryName == ItemSubCategories.BuildingHammer)
        {
            RaycastBuidingDirectionMarkers();
        }
        else
        {
            //When Hammer isn't in the hand anymore
            if ((blockDirection_A != BlockCompass.None && blockDirection_B != BlockDirection.None)
                || blockDirection_A != BlockCompass.None
                || blockDirection_B != BlockDirection.None)
            {
                blockDirection_A = BlockCompass.None;
                blockDirection_B = BlockDirection.None;
                SetAllGhostState_Off();

                if (ghost_PointedAt != null)
                {
                    ghost_PointedAt.SetActive(false);
                    ghost_PointedAt = null;
                }
            }
        }
    }
    void RaycastBuidingDirectionMarkers()
    {
        Ray ray;
        RaycastHit hit;

        if (MainManager.instance.menuStates == MenuStates.None)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            oldRay = ray;
        }

        if (Physics.Raycast(oldRay, out hit))
        {
            //Get the Transform of GameObject hit
            var hitTransform = hit.transform;

            if (hitTransform.tag != "Player")
            {
                BlockTagName = hitTransform.tag;
            }

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
                    switch (hitTransform.gameObject.GetComponent<BuildingBlockDirection>().blockDirection_A)
                    {
                        case BlockCompass.None:
                            break;

                        case BlockCompass.North:
                            if (blockDirection_A != BlockCompass.North)
                            {
                                blockDirection_A = BlockCompass.North;
                            }
                            break;
                        case BlockCompass.East:
                            if (blockDirection_A != BlockCompass.East)
                            {
                                blockDirection_A = BlockCompass.East;
                            }
                            break;
                        case BlockCompass.South:
                            if (blockDirection_A != BlockCompass.South)
                            {
                                blockDirection_A = BlockCompass.South;
                            }
                            break;
                        case BlockCompass.West:
                            if (blockDirection_A != BlockCompass.West)
                            {
                                blockDirection_A = BlockCompass.West;
                            }
                            break;
                        case BlockCompass.Cross_A:
                            if (blockDirection_A != BlockCompass.Cross_A)
                            {
                                blockDirection_A = BlockCompass.Cross_A;
                            }
                            break;
                        case BlockCompass.Cross_B:
                            if (blockDirection_A != BlockCompass.Cross_B)
                            {
                                blockDirection_A = BlockCompass.Cross_B;
                            }
                            break;

                        default:
                            break;
                    }
                    switch (hitTransform.gameObject.GetComponent<BuildingBlockDirection>().blockDirection_B)
                    {
                        case BlockDirection.None:
                            break;

                        case BlockDirection.Up:
                            if (blockDirection_B != BlockDirection.Up)
                            {
                                blockDirection_B = BlockDirection.Up;
                            }
                            break;
                        case BlockDirection.Right:
                            if (blockDirection_B != BlockDirection.Right)
                            {
                                blockDirection_B = BlockDirection.Right;
                            }
                            break;
                        case BlockDirection.Down:
                            if (blockDirection_B != BlockDirection.Down)
                            {
                                blockDirection_B = BlockDirection.Down;
                            }
                            break;
                        case BlockDirection.Left:
                            if (blockDirection_B != BlockDirection.Left)
                            {
                                blockDirection_B = BlockDirection.Left;
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

                //If raycarsting is not on a BuidingDirectionMarkers or ghostBlock
                if ((blockDirection_A != BlockCompass.None
                    && blockDirection_B != BlockDirection.None)
                    || blockDirection_A != BlockCompass.None
                    || blockDirection_B != BlockDirection.None)
                {
                    blockDirection_A = BlockCompass.None;
                    blockDirection_B = BlockDirection.None;
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
            if ((blockDirection_A != BlockCompass.None
                && blockDirection_B != BlockDirection.None)
                || blockDirection_A != BlockCompass.None
                || blockDirection_B != BlockDirection.None)
            {
                blockDirection_A = BlockCompass.None;
                blockDirection_B = BlockDirection.None;
                SetAllGhostState_Off();

                if (ghost_PointedAt != null)
                {
                    ghost_PointedAt.SetActive(false);
                    ghost_PointedAt = null;
                }
            }
        }
    }

    void FindGhostDirection(BuildingBlock_Parent blockLookingAt)
    {
        //blockDirection_A
        if (blockDirection_A == BlockCompass.North && blockDirection_B == BlockDirection.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.North, BlockDirection.None);
        }
        else if (blockDirection_A == BlockCompass.East && blockDirection_B == BlockDirection.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.East, BlockDirection.None);
        }
        else if (blockDirection_A == BlockCompass.South && blockDirection_B == BlockDirection.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.South, BlockDirection.None);
        }
        else if (blockDirection_A == BlockCompass.West && blockDirection_B == BlockDirection.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.West, BlockDirection.None);
        }
        else if (blockDirection_A == BlockCompass.Cross_A && blockDirection_B == BlockDirection.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.Cross_A, BlockDirection.None);
        }
        else if (blockDirection_A == BlockCompass.Cross_B && blockDirection_B == BlockDirection.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.Cross_B, BlockDirection.None);
        }

        //blockDirection_B
        if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.None, BlockDirection.Up);
        }
        else if (blockDirection_B == BlockDirection.Right && blockDirection_A == BlockCompass.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.None, BlockDirection.Right);
        }
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.None, BlockDirection.Down);
        }
        else if (blockDirection_B == BlockDirection.Left && blockDirection_A == BlockCompass.None)
        {
            GhostSelection(blockLookingAt, BlockCompass.None, BlockDirection.Left);
        }

        //Up...
        else if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.North)
        {
            GhostSelection(blockLookingAt, BlockCompass.North, BlockDirection.Up);
        }
        else if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.East)
        {
            GhostSelection(blockLookingAt, BlockCompass.East, BlockDirection.Up);
        }
        else if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.South)
        {
            GhostSelection(blockLookingAt, BlockCompass.South, BlockDirection.Up);
        }
        else if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.West)
        {
            GhostSelection(blockLookingAt, BlockCompass.West, BlockDirection.Up);
        }
        else if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.Cross_A)
        {
            GhostSelection(blockLookingAt, BlockCompass.Cross_A, BlockDirection.Up);
        }
        else if (blockDirection_B == BlockDirection.Up && blockDirection_A == BlockCompass.Cross_B)
        {
            GhostSelection(blockLookingAt, BlockCompass.Cross_B, BlockDirection.Up);
        }

        //Down...
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.North)
        {
            GhostSelection(blockLookingAt, BlockCompass.North, BlockDirection.Down);
        }
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.East)
        {
            GhostSelection(blockLookingAt, BlockCompass.East, BlockDirection.Down);
        }
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.South)
        {
            GhostSelection(blockLookingAt, BlockCompass.South, BlockDirection.Down);
        }
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.West)
        {
            GhostSelection(blockLookingAt, BlockCompass.West, BlockDirection.Down);
        }
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.Cross_A)
        {
            GhostSelection(blockLookingAt, BlockCompass.Cross_A, BlockDirection.Down);
        }
        else if (blockDirection_B == BlockDirection.Down && blockDirection_A == BlockCompass.Cross_B)
        {
            GhostSelection(blockLookingAt, BlockCompass.Cross_B, BlockDirection.Down);
        }

        //Right...
        else if (blockDirection_B == BlockDirection.Right && blockDirection_A == BlockCompass.North)
        {
            GhostSelection(blockLookingAt, BlockCompass.North, BlockDirection.Right);
        }
        else if (blockDirection_B == BlockDirection.Right && blockDirection_A == BlockCompass.East)
        {
            GhostSelection(blockLookingAt, BlockCompass.East, BlockDirection.Right);
        }
        else if (blockDirection_B == BlockDirection.Right && blockDirection_A == BlockCompass.South)
        {
            GhostSelection(blockLookingAt, BlockCompass.South, BlockDirection.Right);
        }
        else if (blockDirection_B == BlockDirection.Right && blockDirection_A == BlockCompass.West)
        {
            GhostSelection(blockLookingAt, BlockCompass.West, BlockDirection.Right);
        }

        //Left...
        else if (blockDirection_B == BlockDirection.Left && blockDirection_A == BlockCompass.North)
        {
            GhostSelection(blockLookingAt, BlockCompass.North, BlockDirection.Left);
        }
        else if (blockDirection_B == BlockDirection.Left && blockDirection_A == BlockCompass.East)
        {
            GhostSelection(blockLookingAt, BlockCompass.East, BlockDirection.Left);
        }
        else if (blockDirection_B == BlockDirection.Left && blockDirection_A == BlockCompass.South)
        {
            GhostSelection(blockLookingAt, BlockCompass.South, BlockDirection.Left);
        }
        else if (blockDirection_B == BlockDirection.Left && blockDirection_A == BlockCompass.West)
        {
            GhostSelection(blockLookingAt, BlockCompass.West, BlockDirection.Left);
        }
    }
    void GhostSelection(BuildingBlock_Parent blockLookingAt, BlockCompass blockCompass, BlockDirection blockDirection)
    {
        for (int i = 0; i < blockLookingAt.ghostList.Count; i++)
        {
            if (blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == blockCompass
                && blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().blockDirection_B == blockDirection)
            {
                SetGhostState_ON(blockLookingAt, i);
            }
            else
            {
                SetGhostState_OFF(blockLookingAt, i);
            }
        }
    }

    void SetGhostState_ON(BuildingBlock_Parent blockLookingAt, int i)
    {
        //Floor
        if (buildingType_Selected == BuildingType.Floor)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(blockLookingAt, i, BuildingType.Floor, BuildingSubType.None, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Floor - triangle
        else if (buildingType_Selected == BuildingType.Triangle)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(blockLookingAt, i, BuildingType.Triangle, BuildingSubType.None, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Wall_Diagonal
        else if (buildingType_Selected == BuildingType.Wall && (blockLookingAt.buildingSubType == BuildingSubType.Wall_Diagonaly || blockDirection_A == BlockCompass.Cross_A || blockDirection_A == BlockCompass.Cross_B))
        {
            //Wood
            BuidingBlockCanBePlacedCheck(blockLookingAt, i, BuildingType.Wall, BuildingSubType.Wall_Diagonaly, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }
        
        //Wall
        else if (buildingType_Selected == BuildingType.Wall && blockLookingAt.buildingSubType == BuildingSubType.None)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(blockLookingAt, i, BuildingType.Wall, BuildingSubType.None, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Angled
        else if (buildingType_Selected == BuildingType.Angeled)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(blockLookingAt, i, BuildingType.Angeled, BuildingSubType.None, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

            //Stone


            //Iron


        }

        //Turn off
        else
        {
            SetGhostState_OFF(blockLookingAt, i);
        }
    }
    void SetGhostState_OFF(BuildingBlock_Parent blockLookingAt, int i)
    {
        if (blockLookingAt!= null)
        {
            blockLookingAt.ghostList[i].SetActive(false);
            blockLookingAt.ghostList[i].GetComponent<MeshRenderer>().material = invisible_Material;
            blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().isSelected = false;
        }
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

    void BuidingBlockCanBePlacedCheck(BuildingBlock_Parent blockLookingAt, int i, BuildingType buildingType, BuildingSubType buildingSubType, Material material_Can, Material material_Cannot)
    {
        if (blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().buildingType != buildingType
            || blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().buildingSubType != buildingSubType)
        {
            print("Return!!!");
            return;
        }

        //Reset all ghost before setting a new one
        if (ghost_PointedAt != blockLookingAt.ghostList[i])
        {
            SetAllGhostState_Off();
            ghost_PointedAt = blockLookingAt.ghostList[i];
        }

        //print("CheckOverlappingGhost(): " + CheckOverlappingGhost() + " | BuildingType: " + blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().buildingType + " = " + buildingType);

        //Can be placed
        if (!CheckOverlappingGhost())
        {
            blockLookingAt.ghostList[i].GetComponent<MeshRenderer>().material = material_Can;
            blockLookingAt.ghostList[i].GetComponent<Building_Ghost>().isSelected = true;
            blockLookingAt.ghostList[i].SetActive(true);
            buildingBlockCanBePlaced = true;
        }

        //Cannot be placed
        else
        {
            //Insert functionality to prevent a selected ghost to be placed, and change its color to red
            //Or maybe not?! - To decrease the Materials needed to be made
            #region
            //if()...
            //buildingBlock.ghostList[i].GetComponent<MeshRenderer>().material = cannotPlace_Material;
            //buildingBlock.ghostList[i].GetComponent<Building_Ghost>().isSelected = true;
            //buildingBlock.ghostList[i].SetActive(true);
            //buildingBlockCanBePlaced = false;
            #endregion

            buildingBlockCanBePlaced = false;
            SetAllGhostState_Off();
            //SetGhostState_OFF(buildingBlock, i);
        }
    }
    bool CheckOverlappingGhost()
    {
        if (lastBuildingBlock_LookedAt == null || lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>() == null || lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList == null || lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().ghostList == null)
        {
            return false;
        }

        //If Wall is selected
        if (buildingType_Selected == BuildingType.Wall && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.None)
        {
            for (int i = 0; i < lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList.Count; i++)
            {
                for (int j = 0; j < lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList.Count; j++)
                {
                    //North - Up
                    if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && blockDirection_A == BlockCompass.North && blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }

                    //North - Down
                    else if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && blockDirection_A == BlockCompass.North && blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }

                    //South - Up
                    if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && blockDirection_A == BlockCompass.South && blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }

                    //South - Down
                    else if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && blockDirection_A == BlockCompass.South && blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }
                }
            }
        }

        //If Wall_Diagonaly is selected
        else if (buildingType_Selected == BuildingType.Wall && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly)
        {
            for (int i = 0; i < lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList.Count; i++)
            {
                for (int j = 0; j < lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().ghostList.Count; j++)
                {
                    //North - Up
                    if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly

                        && blockDirection_A == BlockCompass.North && blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }

                    //North - Down
                    else if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly

                        && blockDirection_A == BlockCompass.North && blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }

                    //South - Up
                    if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly

                        && blockDirection_A == BlockCompass.South && blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Up
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }

                    //South - Down
                    else if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.Wall_Diagonaly

                        && blockDirection_A == BlockCompass.South && blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Down
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[i].buildingBlock.transform.rotation == lastBuildingBlock_LookedAt.transform.rotation)
                    {
                        SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), j);
                        return true;
                    }
                }
            }
        }

        //If any other BuildingBlock is selected
        else
        {
            for (int j = 0; j < lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList.Count; j++)
            {
                for (int k = 0; k < lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().ghostList.Count; k++)
                {
                    if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[j].buildingBlock.transform.position == lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().ghostList[k].transform.position
                        && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[j].buildingBlock.transform.position == ghost_PointedAt.transform.position)
                    {
                        //If lastBuildingBlock_LookedAt is Floor or Triangle
                        if ((lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[j].buildingType == BuildingType.Floor
                                || lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[j].buildingType == BuildingType.Triangle)
                            && (buildingType_Selected == BuildingType.Floor || buildingType_Selected == BuildingType.Triangle))
                        {
                            print("3. End Return");
                            SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), k);
                            return true;
                        }

                        //If lastBuildingBlock_LookedAt is any other Block
                        else if (lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList[j].buildingType == buildingType_Selected)
                        {
                            print("4. End Return");
                            SetGhostState_OFF(lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>(), k);
                            return true;
                        }
                    }
                }
            }
        }

        print("5. End Return");
        return false;
    }


    //--------------------


    public void PlaceBlock()
    {
        if (ghost_PointedAt != null && buildingBlockCanBePlaced)
        {
            print("1. Place Block");

            //Play Sound
            if (buildingMaterial_Selected == BuildingMaterial.Wood)
            {
                SoundManager.instance.PlayWood_Placed_Clip();
            }
            else if (buildingMaterial_Selected == BuildingMaterial.Stone)
            {
                SoundManager.instance.PlayStone_Placed_Clip();
            }
            else if (buildingMaterial_Selected == BuildingMaterial.Iron)
            {
                SoundManager.instance.PlayIron_Placed_Clip();
            }

            //SetRotation of BuildingBlock
            Quaternion rotation = new Quaternion(ghost_PointedAt.transform.rotation.x, ghost_PointedAt.transform.rotation.y, ghost_PointedAt.transform.rotation.z, ghost_PointedAt.transform.rotation.w);

            #region Place correct BuildingBlock and Material
            //Floor
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Floor && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Floor");
                    buildingBlockList.Add(Instantiate(builingBlock_Wood_Floor, ghost_PointedAt.transform.position, rotation) as GameObject);
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    print("Placed: Stone Floor");
                    buildingBlockList.Add(Instantiate(builingBlock_Stone_Floor, ghost_PointedAt.transform.position, rotation) as GameObject);

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Floor");
                    buildingBlockList.Add(Instantiate(builingBlock_Iron_Floor, ghost_PointedAt.transform.position, rotation) as GameObject);

                }
            }

            //Floor - Triangle
            else if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Triangle && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Triangle");
                    buildingBlockList.Add(Instantiate(builingBlock_Wood_Floor_Triangle, ghost_PointedAt.transform.position, rotation) as GameObject);
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    print("Placed: Stone Triangle");
                    buildingBlockList.Add(Instantiate(builingBlock_Stone_Floor_Triangle, ghost_PointedAt.transform.position, rotation) as GameObject);

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Triangle");
                    buildingBlockList.Add(Instantiate(builingBlock_Iron_Floor_Triangle, ghost_PointedAt.transform.position, rotation) as GameObject);

                }
            }

            //Wall_Diagonal
            else if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall && ghost_PointedAt.GetComponent<Building_Ghost>().buildingSubType == BuildingSubType.Wall_Diagonaly && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                BlockCompass a = ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A;
                BlockDirection b = ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_B;

                if ((a == BlockCompass.North && b == BlockDirection.Left) || (a == BlockCompass.South && b == BlockDirection.Left) || (a == BlockCompass.North && b == BlockDirection.Right) || (a == BlockCompass.South && b == BlockDirection.Right))
                {
                    //Wood
                    if (buildingMaterial_Selected == BuildingMaterial.Wood)
                    {
                        print("Placed: Wood Wall_Diagonal");
                        buildingBlockList.Add(Instantiate(builingBlock_Wood_Wall, ghost_PointedAt.transform.position, rotation) as GameObject);
                    }

                    //Stone
                    else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                    {
                        print("Placed: Stone Wall_Diagonal");
                        buildingBlockList.Add(Instantiate(builingBlock_Stone_Wall, ghost_PointedAt.transform.position, rotation) as GameObject);

                    }

                    //Iron
                    else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                    {
                        print("Placed: Iron Wall_Diagonal");
                        buildingBlockList.Add(Instantiate(builingBlock_Iron_Wall, ghost_PointedAt.transform.position, rotation) as GameObject);

                    }
                }
                else
                {
                    //Wood
                    if (buildingMaterial_Selected == BuildingMaterial.Wood)
                    {
                        print("Placed: Wood Wall_Diagonal");
                        buildingBlockList.Add(Instantiate(builingBlock_Wood_Wall_Diagonal, ghost_PointedAt.transform.position, rotation) as GameObject);
                    }

                    //Stone
                    else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                    {
                        print("Placed: Stone Wall_Diagonal");
                        buildingBlockList.Add(Instantiate(builingBlock_Stone_Wall_Diagonal, ghost_PointedAt.transform.position, rotation) as GameObject);

                    }

                    //Iron
                    else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                    {
                        print("Placed: Iron Wall_Diagonal");
                        buildingBlockList.Add(Instantiate(builingBlock_Iron_Wall_Diagonal, ghost_PointedAt.transform.position, rotation) as GameObject);

                    }
                }
            }

            //Wall
            else if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall && ghost_PointedAt.GetComponent<Building_Ghost>().buildingSubType == BuildingSubType.None && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Wall");
                    buildingBlockList.Add(Instantiate(builingBlock_Wood_Wall, ghost_PointedAt.transform.position, rotation) as GameObject);
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    print("Placed: Stone Wall");
                    buildingBlockList.Add(Instantiate(builingBlock_Stone_Wall, ghost_PointedAt.transform.position, rotation) as GameObject);

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Wall");
                    buildingBlockList.Add(Instantiate(builingBlock_Iron_Wall, ghost_PointedAt.transform.position, rotation) as GameObject);

                }
            }

            //Angle
            else if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Angeled && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood Angle");
                    buildingBlockList.Add(Instantiate(builingBlock_Wood_Angle, ghost_PointedAt.transform.position, rotation) as GameObject);
                }

                //Stone
                else if (buildingMaterial_Selected == BuildingMaterial.Stone)
                {
                    print("Placed: Stone Angle");
                    buildingBlockList.Add(Instantiate(builingBlock_Stone_Angle, ghost_PointedAt.transform.position, rotation) as GameObject);

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Angle");
                    buildingBlockList.Add(Instantiate(builingBlock_Iron_Angle, ghost_PointedAt.transform.position, rotation) as GameObject);

                }
            }


            //Angle_Corner


            //Stair


            //Window


            //Fence


            //Fence Angle


            //Door

            #endregion

            //Set Parent of the placed object
            buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);

            #region Setup the Placed Block
            //Set info on the Placed Block
            BlockPlaced blockPlaced = new BlockPlaced();
            blockPlaced.buildingBlock = lastBuildingBlock_LookedAt;
            blockPlaced.buildingType = lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingType;
            blockPlaced.buildingSubType = lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType;
            buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().isStrange = ghost_PointedAt.GetComponent<Building_Ghost>().isStrange;
            buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().blockPlacedList.Add(blockPlaced);

            #endregion
            #region Setup the Block that got a Block Placed on It
            //Set info on the Block that got a block placed on it
            BlockPlaced blockGotPlacedOn = new BlockPlaced();
            blockGotPlacedOn.buildingBlock = buildingBlockList[buildingBlockList.Count - 1];
            blockGotPlacedOn.buildingType = buildingType_Selected;
            blockGotPlacedOn.buildingSubType = buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().buildingSubType;
            lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().blockPlacedList.Add(blockGotPlacedOn);
            #endregion

            #region Update all other Blocks that are Adjacent to the placed Block
            //Insert the placed block on all adjacent other buildingBlockLists
            if (buildingType_Selected == BuildingType.Wall && lastBuildingBlock_LookedAt.GetComponent<BuildingBlock_Parent>().buildingSubType == BuildingSubType.None)
            {
                
            }
            else
            {
                for (int i = 0; i < buildingBlockList.Count; i++)
                {
                    for (int j = 0; j < buildingBlockList[i].GetComponent<BuildingBlock_Parent>().ghostList.Count; j++)
                    {
                        if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().ghostList[j].transform.position == buildingBlockList[buildingBlockList.Count - 1].transform.position
                            && buildingBlockList[i].GetComponent<BuildingBlock_Parent>().ghostList[j].GetComponent<Building_Ghost>().buildingType == buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().buildingType)
                        {
                            int amount = 0;

                            for (int k = 0; k < buildingBlockList[i].GetComponent<BuildingBlock_Parent>().blockPlacedList.Count; k++)
                            {
                                if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().blockPlacedList[k].buildingBlock == buildingBlockList[buildingBlockList.Count - 1])
                                {
                                    amount++;
                                }
                            }

                            if (amount <= 0)
                            {
                                buildingBlockList[i].GetComponent<BuildingBlock_Parent>().blockPlacedList.Add(blockGotPlacedOn);
                            }
                        }
                    }
                }
            }
            

            #endregion
            #region Insert info about Adjacent Blocks on the Placed Block
            for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
            {
                for (int j = 0; j < buildingBlockList.Count; j++)
                {
                    if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].transform.position == buildingBlockList[j].transform.position
                        && buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().buildingType == buildingBlockList[j].GetComponent<BuildingBlock_Parent>().buildingType)
                    {
                        int amount = 0;

                        for (int k = 0; k < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().blockPlacedList.Count; k++)
                        {
                            if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().blockPlacedList[k].buildingBlock == buildingBlockList[j])
                            {
                                amount++;
                            }
                        }

                        if (amount <= 0)
                        {
                            if (buildingType_Selected == BuildingType.Wall && buildingBlockList[j].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall)
                            {
                                BlockPlaced blockAdjacent = new BlockPlaced();
                                blockAdjacent.buildingBlock = buildingBlockList[j];
                                blockAdjacent.buildingType = buildingBlockList[j].GetComponent<BuildingBlock_Parent>().buildingType;

                                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().blockPlacedList.Add(blockAdjacent);
                            }
                            else
                            {
                                BlockPlaced blockAdjacent = new BlockPlaced();
                                blockAdjacent.buildingBlock = buildingBlockList[j];
                                blockAdjacent.buildingType = buildingBlockList[j].GetComponent<BuildingBlock_Parent>().buildingType;

                                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().blockPlacedList.Add(blockAdjacent);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Block exclude Itself from its list
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                for (int j = 0; j < buildingBlockList[i].GetComponent<BuildingBlock_Parent>().blockPlacedList.Count; j++)
                {
                    if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingSubType != BuildingSubType.Wall_Diagonaly)
                    {
                        if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().blockPlacedList[j].buildingBlock == buildingBlockList[i])
                        {
                            buildingBlockList[i].GetComponent<BuildingBlock_Parent>().blockPlacedList.RemoveAt(j);
                            break;
                        }
                    }
                }
            }
            #endregion
        }

        //Reset parameters
        lastBuildingBlock_LookedAt = null;
        buildingBlockCanBePlaced = false;
        SetAllGhostState_Off();
    }
    void RotateBlock()
    {
        if (ghost_PointedAt == null)
        {
            return;
        }

        print("Rotate");

        BuildingBlock_Parent parent = ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.GetComponent<BuildingBlock_Parent>();

        if (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North)
        {
            for (int i = 0; i < parent.ghostList.Count; i++)
            {
                if (parent.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.East)
                {
                    ghost_PointedAt = parent.ghostList[i];
                    return;
                }
            }
            
        }
        else if (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.East)
        {
            for (int i = 0; i < parent.ghostList.Count; i++)
            {
                if (parent.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South)
                {
                    ghost_PointedAt = parent.ghostList[i];
                    return;
                }
            }

        }
        else if (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South)
        {
            for (int i = 0; i < parent.ghostList.Count; i++)
            {
                if (parent.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.West)
                {
                    ghost_PointedAt = parent.ghostList[i];
                    return;
                }
            }

        }
        else if (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.West)
        {
            for (int i = 0; i < parent.ghostList.Count; i++)
            {
                if (parent.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North)
                {
                    ghost_PointedAt = parent.ghostList[i];
                    return;
                }
            }

        }
        else if (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.Cross_A)
        {
            for (int i = 0; i < parent.ghostList.Count; i++)
            {
                if (parent.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.Cross_B)
                {
                    ghost_PointedAt = parent.ghostList[i];
                    return;
                }
            }

        }
        else if (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.Cross_B)
        {
            for (int i = 0; i < parent.ghostList.Count; i++)
            {
                if (parent.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.Cross_A)
                {
                    ghost_PointedAt = parent.ghostList[i];
                    return;
                }
            }

        }
    }
}
