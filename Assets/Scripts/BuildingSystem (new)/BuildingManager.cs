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
    [SerializeField] Vector2 BuildingDistance;
    public BlockCompass buildingBlockDirection_Selected_A;
    public BlockDirection buildingBlockDirection_Selected_B;
    public BuildingType buildingType_Selected = BuildingType.None;
    public BuildingMaterial buildingMaterial_Selected = BuildingMaterial.None;

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
            if ((buildingBlockDirection_Selected_A != BlockCompass.None
                && buildingBlockDirection_Selected_B != BlockDirection.None)
                || buildingBlockDirection_Selected_A != BlockCompass.None
                || buildingBlockDirection_Selected_B != BlockDirection.None)
            {
                buildingBlockDirection_Selected_A = BlockCompass.None;
                buildingBlockDirection_Selected_B = BlockDirection.None;
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
                            if (buildingBlockDirection_Selected_A != BlockCompass.North)
                            {
                                buildingBlockDirection_Selected_A = BlockCompass.North;
                            }
                            break;
                        case BlockCompass.East:
                            if (buildingBlockDirection_Selected_A != BlockCompass.East)
                            {
                                buildingBlockDirection_Selected_A = BlockCompass.East;
                            }
                            break;
                        case BlockCompass.South:
                            if (buildingBlockDirection_Selected_A != BlockCompass.South)
                            {
                                buildingBlockDirection_Selected_A = BlockCompass.South;
                            }
                            break;
                        case BlockCompass.West:
                            if (buildingBlockDirection_Selected_A != BlockCompass.West)
                            {
                                buildingBlockDirection_Selected_A = BlockCompass.West;
                            }
                            break;
                        case BlockCompass.Cross_A:
                            if (buildingBlockDirection_Selected_A != BlockCompass.Cross_A)
                            {
                                buildingBlockDirection_Selected_A = BlockCompass.Cross_A;
                            }
                            break;
                        case BlockCompass.Cross_B:
                            if (buildingBlockDirection_Selected_A != BlockCompass.Cross_B)
                            {
                                buildingBlockDirection_Selected_A = BlockCompass.Cross_B;
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
                            if (buildingBlockDirection_Selected_B != BlockDirection.Up)
                            {
                                buildingBlockDirection_Selected_B = BlockDirection.Up;
                            }
                            break;
                        case BlockDirection.Right:
                            if (buildingBlockDirection_Selected_B != BlockDirection.Right)
                            {
                                buildingBlockDirection_Selected_B = BlockDirection.Right;
                            }
                            break;
                        case BlockDirection.Down:
                            if (buildingBlockDirection_Selected_B != BlockDirection.Down)
                            {
                                buildingBlockDirection_Selected_B = BlockDirection.Down;
                            }
                            break;
                        case BlockDirection.Left:
                            if (buildingBlockDirection_Selected_B != BlockDirection.Left)
                            {
                                buildingBlockDirection_Selected_B = BlockDirection.Left;
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
                if ((buildingBlockDirection_Selected_A != BlockCompass.None
                    && buildingBlockDirection_Selected_B != BlockDirection.None)
                    || buildingBlockDirection_Selected_A != BlockCompass.None
                    || buildingBlockDirection_Selected_B != BlockDirection.None)
                {
                    buildingBlockDirection_Selected_A = BlockCompass.None;
                    buildingBlockDirection_Selected_B = BlockDirection.None;
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
            if ((buildingBlockDirection_Selected_A != BlockCompass.None
                && buildingBlockDirection_Selected_B != BlockDirection.None)
                || buildingBlockDirection_Selected_A != BlockCompass.None
                || buildingBlockDirection_Selected_B != BlockDirection.None)
            {
                buildingBlockDirection_Selected_A = BlockCompass.None;
                buildingBlockDirection_Selected_B = BlockDirection.None;
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
        switch (buildingBlockDirection_Selected_A)
        {
            case BlockCompass.None:
                break;

            case BlockCompass.North:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.North)
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
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.East)
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
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.South)
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
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.West)
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
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.Cross_A)
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
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_A == BlockCompass.Cross_B)
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
        switch (buildingBlockDirection_Selected_B)
        {
            case BlockDirection.None:
                break;

            case BlockDirection.Up:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Up)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockDirection.Right:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Right)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockDirection.Down:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Down)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockDirection.Left:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection_B == BlockDirection.Left)
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
        if (buildingBlock!= null)
        {
            buildingBlock.ghostList[i].SetActive(false);
            buildingBlock.ghostList[i].GetComponent<MeshRenderer>().material = invisible_Material;
            buildingBlock.ghostList[i].GetComponent<Building_Ghost>().isSelected = false;
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

        print("CheckOverlappingGhost(): " + CheckOverlappingGhost() + " | BuildingType: " + buildingBlock.ghostList[i].GetComponent<Building_Ghost>().buildingType + " = " + buildingType);

        //Can be placed
        if (!CheckOverlappingGhost()
            && buildingBlock.ghostList[i].GetComponent<Building_Ghost>().buildingType == buildingType)
        {
            buildingBlock.ghostList[i].GetComponent<MeshRenderer>().material = material_Can;
            buildingBlock.ghostList[i].GetComponent<Building_Ghost>().isSelected = true;
            buildingBlock.ghostList[i].SetActive(true);
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
        //Make new Vctor3's
        Vector3 tempSouth = new Vector3();
        Vector3 tempNorth = new Vector3();
        Vector3 tempEast = new Vector3();
        Vector3 tempWest = new Vector3();

        Vector3 tempCross = new Vector3();
        //Vector3 tempCross_A = new Vector3();
        //Vector3 tempCross_B = new Vector3();

        Vector3 tempUp = new Vector3();
        Vector3 tempRight = new Vector3();
        Vector3 tempDown = new Vector3();
        Vector3 tempLeft = new Vector3();


        //-----


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
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                //Set Direction Restrictions
                //if (buildingBlockDirection_Selected_A == BlockCompass.South)
                //{
                //    tempSouth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                //    tempNorth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);
                //}
                //else if (buildingBlockDirection_Selected_A == BlockCompass.North)
                //{
                //    tempSouth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);
                //    tempNorth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                //}
                //else if (buildingBlockDirection_Selected_A == BlockCompass.West)
                //{
                //    tempEast = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                //    tempWest = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                //}
                //else if (buildingBlockDirection_Selected_A == BlockCompass.East)
                //{
                //    tempEast = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                //    tempWest = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                //}

                //if (buildingBlockDirection_Selected_B == BlockDirection.Up)
                //{
                //    tempUp = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y - 2, buildingBlockList[i].transform.position.z);
                //}
                //else if (buildingBlockDirection_Selected_B == BlockDirection.Down)
                //{
                //    tempDown = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y + 2, buildingBlockList[i].transform.position.z);
                //}

                //Set Ghost False if reqirement is met
                if (buildingBlockDirection_Selected_A == BlockCompass.West
                    && buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                    && ghost_PointedAt.transform.position == buildingBlockList[i].transform.position
                    && (ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == buildingBlockList[i].transform.rotation.y
                        || ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == -buildingBlockList[i].transform.rotation.y))
                {
                    print("1. Easy");

                    SetAllGhostState_Off();
                    return true;
                }
                else if (buildingBlockDirection_Selected_A == BlockCompass.East
                    && buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                    && ghost_PointedAt.transform.position == buildingBlockList[i].transform.position
                    && (ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == buildingBlockList[i].transform.rotation.y
                        || ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == -buildingBlockList[i].transform.rotation.y))
                {
                    print("2. Easy");

                    SetAllGhostState_Off();
                    return true;
                }
                else if (buildingBlockDirection_Selected_B == BlockDirection.Up
                    && buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                    && ghost_PointedAt.transform.position == buildingBlockList[i].transform.position
                    && (ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == buildingBlockList[i].transform.rotation.y
                        || ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == -buildingBlockList[i].transform.rotation.y))
                {
                    print("3. Easy");

                    SetAllGhostState_Off();
                    return true;
                }
                else if (buildingBlockDirection_Selected_B == BlockDirection.Down
                    && buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall
                    && ghost_PointedAt.transform.position == buildingBlockList[i].transform.position
                    && (ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == buildingBlockList[i].transform.rotation.y
                        || ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.rotation.y == -buildingBlockList[i].transform.rotation.y))
                {
                    print("4. Easy");

                    SetAllGhostState_Off();
                    return true;
                }
            }
        }

        //Wall - Diagonally
        else if (buildingType_Selected == BuildingType.Wall_Diagonaly)
        {
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall_Diagonaly
                    && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == buildingBlockList[i].transform.position)
                {
                    SetAllGhostState_Off();
                    return true;
                }
            }
        }

        //Angeled
        else if (buildingType_Selected == BuildingType.Angeled)
        {
            for (int i = 0; i < buildingBlockList.Count; i++)
            {
                //Set Direction Restrictions
                if (buildingBlockDirection_Selected_A == BlockCompass.South)
                {
                    tempSouth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);
                    tempNorth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);
                    tempEast = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);
                    tempWest = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);

                    tempCross = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z + 2);
                }
                else if (buildingBlockDirection_Selected_A == BlockCompass.North)
                {
                    tempSouth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);
                    tempNorth = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);
                    tempEast = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);
                    tempWest = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);

                    tempCross = new Vector3(buildingBlockList[i].transform.position.x, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z - 2);
                }
                else if (buildingBlockDirection_Selected_A == BlockCompass.West)
                {
                    tempSouth = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    tempNorth = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    tempEast = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    tempWest = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);

                    tempCross = new Vector3(buildingBlockList[i].transform.position.x + 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                }
                else if (buildingBlockDirection_Selected_A == BlockCompass.East)
                {
                    tempSouth = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    tempNorth = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    tempEast = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                    tempWest = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);

                    tempCross = new Vector3(buildingBlockList[i].transform.position.x - 2, buildingBlockList[i].transform.position.y, buildingBlockList[i].transform.position.z);
                }

                //Set Ghost False if reqirement is met
                //Angled
                if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Angeled
                    && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced_A == BlockCompass.South)
                    && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempSouth)
                {
                    SetAllGhostState_Off();
                    return true;
                }
                else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Angeled
                    && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced_A == BlockCompass.North)
                    && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempNorth)
                {
                    SetAllGhostState_Off();
                    return true;
                }
                else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Angeled
                    && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced_A == BlockCompass.East)
                    && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempEast)
                {
                    SetAllGhostState_Off();
                    return true;
                }
                else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Angeled
                    && (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().directionPlaced_A == BlockCompass.West)
                    && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempWest)
                {
                    SetAllGhostState_Off();
                    return true;
                }

                //Wall_Diagonally
                else if (buildingBlockList[i].GetComponent<BuildingBlock_Parent>().buildingType == BuildingType.Wall_Diagonaly
                    && ghost_PointedAt.GetComponent<Building_Ghost>().blockParent.transform.position == tempCross)
                {
                    SetAllGhostState_Off();
                    return true;
                }
            }
        }

        return false;
    }


    //--------------------


    public void PlaceBlock()
    {
        if (ghost_PointedAt != null && buildingBlockCanBePlaced)
        {
            print("1. Place Block");

            //SetRotation of BuildingBlock
            Quaternion rotation = new Quaternion(ghost_PointedAt.transform.rotation.x, ghost_PointedAt.transform.rotation.y, ghost_PointedAt.transform.rotation.z, ghost_PointedAt.transform.rotation.w);

            #region Place correct BuildingBlock
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

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Floor");

                }
            }

            //Floor - Triangle
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Triangle && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
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

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Triangle");

                }
            }

            //Wall
            else if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
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

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Wall");

                }
            }

            //Wall_Diagonal
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Wall_Diagonaly && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
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

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Wall_Diagonal");

                }
            }

            //Angle
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Angeled && ghost_PointedAt.GetComponent<Building_Ghost>().isSelected)
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

                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron Angle");

                }
            }
            #endregion

            //Set Parent of placed object
            buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);

            //Set BlockCompass in the placed block
            if (buildingBlockDirection_Selected_A == BlockCompass.North)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_A = BlockCompass.North;
            }
            else if (buildingBlockDirection_Selected_A == BlockCompass.East)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_A = BlockCompass.East;
            }
            else if (buildingBlockDirection_Selected_A == BlockCompass.South)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_A = BlockCompass.South;
            }
            else if (buildingBlockDirection_Selected_A == BlockCompass.West)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_A = BlockCompass.West;
            }

            //Set BlockDirection in the placed block
            if (buildingBlockDirection_Selected_B == BlockDirection.Up)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_B = BlockDirection.Up;
            }
            else if (buildingBlockDirection_Selected_B == BlockDirection.Right)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_B = BlockDirection.Right;
            }
            else if (buildingBlockDirection_Selected_B == BlockDirection.Down)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_B = BlockDirection.Down;
            }
            else if (buildingBlockDirection_Selected_B == BlockDirection.Right)
            {
                buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().directionPlaced_B = BlockDirection.Right;
            }

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
