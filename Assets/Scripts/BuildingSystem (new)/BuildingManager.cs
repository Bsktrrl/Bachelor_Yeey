using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance { get; set; } //Singleton

    public List<GameObject> buildingBlockList = new List<GameObject>();
    public GameObject buildingBlock_Parent;

    [Header("Ghost")]
    public GameObject lastBuildingBlock_PointedAt;
    public GameObject buildingBlock_PointedAt;
    public GameObject ghost_PointedAt;
    public bool buildingBlockCanBePlaced;

    [Header("Selected")]
    public BlockDirection buildingBlockDirection_Selected;
    public BuildingType buildingType_Selected = BuildingType.None;
    public BuildingMaterial buildingMaterial_Selected = BuildingMaterial.None;

    [Header("BuildingBlocks List")]
    [SerializeField] GameObject builingBlock_Floor;

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
        //PlayerButtonManager.leftMouse_isPressedDown += PlaceBlock;
    }
    private void Update()
    {
        //Only active when not in a menu
        if (MainManager.instance.menuStates == MenuStates.None)
        {
            RaycastBuidingDirectionMarkers();

            //RaycastGhost();
            //SetGhostInactive();
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

                //Get the BuildingBlockDirection
                if (hitTransform.gameObject.CompareTag("BuidingDirectionMarkers") || hitTransform.gameObject.CompareTag("BuildingBlock_Ghost"))
                {
                    if (hitTransform.gameObject.CompareTag("BuidingDirectionMarkers"))
                    {
                        switch (hitTransform.gameObject.GetComponent<BuildingBlockDirection>().blockDirection)
                        {
                            case BlockDirection.None:
                                break;

                            case BlockDirection.North:
                                if (buildingBlockDirection_Selected != BlockDirection.North)
                                {
                                    buildingBlockDirection_Selected = BlockDirection.North;
                                    //print("1. North");
                                }
                                break;
                            case BlockDirection.East:
                                if (buildingBlockDirection_Selected != BlockDirection.East)
                                {
                                    buildingBlockDirection_Selected = BlockDirection.East;
                                    //print("2. East");
                                }
                                break;
                            case BlockDirection.South:
                                if (buildingBlockDirection_Selected != BlockDirection.South)
                                {
                                    buildingBlockDirection_Selected = BlockDirection.South;
                                    //print("3. South");
                                }
                                break;
                            case BlockDirection.West:
                                if (buildingBlockDirection_Selected != BlockDirection.West)
                                {
                                    buildingBlockDirection_Selected = BlockDirection.West;
                                    //print("4. West");
                                }
                                break;

                            default:
                                break;
                        }

                        FindGhostDirection(hitTransform.gameObject.GetComponent<BuildingBlockDirection>().parentBlock.GetComponent<BuildingBlock_Parent>());
                    }
                }
                else
                {
                    //If raycarsting is not on a buildingBlock or ghostBlock
                    if (buildingBlockDirection_Selected != BlockDirection.None)
                    {
                        //print("0. BlockDirection.None");
                        buildingBlockDirection_Selected = BlockDirection.None;

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
                if (buildingBlockDirection_Selected != BlockDirection.None)
                {
                    //print("1. BlockDirection.None");
                    buildingBlockDirection_Selected = BlockDirection.None;

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
            if (buildingBlockDirection_Selected != BlockDirection.None)
            {
                //print("2. BlockDirection.None");
                buildingBlockDirection_Selected = BlockDirection.None;

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
            case BlockDirection.None:
                break;

            case BlockDirection.North:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.North)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockDirection.East:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.East)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockDirection.South:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.South)
                    {
                        SetGhostState_ON(buildingBlock, i);
                    }
                    else
                    {
                        SetGhostState_OFF(buildingBlock, i);
                    }
                }
                break;
            case BlockDirection.West:
                for (int i = 0; i < buildingBlock.ghostList.Count; i++)
                {
                    if (buildingBlock.ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.West)
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

        //Wall
        else if (buildingType_Selected == BuildingType.Wall)
        {
            //Wood
            BuidingBlockCanBePlacedCheck(buildingBlock, i, BuildingType.Wall, canPlace_Material, cannotPlace_Material); //Change Material when Mesh is ready

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
                    //print("Placed: Wood");

                    SetAllGhostState_Off();
                    SoundManager.instance.PlayWood_Placed_Clip();

                    buildingBlockList.Add(Instantiate(builingBlock_Floor) as GameObject);
                    buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                    buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                    #region Old
                    //ghost_PointedAt.GetComponent<Building_Ghost>().isDeleted = true;

                    //switch (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection)
                    //{
                    //    case BlockDirection.None:
                    //        break;

                    //    case BlockDirection.North:
                    //        for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                    //        {
                    //            if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.South)
                    //            {
                    //                //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                    //            }
                    //        }
                    //        break;
                    //    case BlockDirection.East:
                    //        for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                    //        {
                    //            if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.West)
                    //            {
                    //                //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                    //            }
                    //        }
                    //        break;
                    //    case BlockDirection.South:
                    //        for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                    //        {
                    //            if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.North)
                    //            {
                    //                //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                    //            }
                    //        }
                    //        break;
                    //    case BlockDirection.West:
                    //        for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                    //        {
                    //            if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.East)
                    //            {
                    //                //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                    //            }
                    //        }
                    //        break;

                    //    default:
                    //        break;
                    //}

                    //ghost_PointedAt.GetComponent<Building_Ghost>().isDeleted = true;
                    #endregion
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

            //Else if (Wall)
                //Wood
                //Stone
                //Iron

            //Else if...
                //Wood
                //Stone
                //Iron
            

        }
    }
}
