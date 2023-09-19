using System.Collections;
using System.Collections.Generic;
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
            RaycastGhost();
            SetGhostInactive();
        }
    }


    //--------------------


    void RaycastGhost()
    {
        if (HandManager.instance.selectedSlotItem.subCategoryName == ItemSubCategories.BuildingHammer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Get the Transform of GameObject hit
                var hitTransform = hit.transform;

                //Update available Blocks to select based on the BuildingBlock
                if (hitTransform.gameObject.CompareTag("BuildingBlock"))
                {
                    //Update buildingBlock_PointedAt
                    buildingBlock_PointedAt = hitTransform.gameObject;
                    lastBuildingBlock_PointedAt = buildingBlock_PointedAt;

                    //Update which Ghost to be active (able to select with ghosts)
                    switch (buildingType_Selected)
                    {
                        case BuildingType.None:
                            SetActiveGhost(BuildingType.None);
                            break;

                        case BuildingType.Floor:
                            SetActiveGhost(BuildingType.Floor);
                            break;
                        case BuildingType.Wall:
                            SetActiveGhost(BuildingType.Wall);
                            break;
                        case BuildingType.Wall_Diagonaly:
                            SetActiveGhost(BuildingType.Wall_Diagonaly);
                            break;
                        case BuildingType.Stair:
                            SetActiveGhost(BuildingType.Stair);
                            break;
                        case BuildingType.Angeled:
                            SetActiveGhost(BuildingType.Angeled);
                            break;
                        case BuildingType.Angeled_Corner:
                            SetActiveGhost(BuildingType.Angeled_Corner);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    //Update buildingBlock_PointedAt
                    buildingBlock_PointedAt = null;
                }

                //Change material when hovering over a selecteable BuildingBlock_Ghost
                if (hitTransform.gameObject.CompareTag("BuildingBlock_Ghost"))
                {
                    //Reset previous selected BuildingBlock_Ghost
                    if (ghost_PointedAt != null)
                    {
                        ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = invisible_Material;
                        ghost_PointedAt.gameObject.GetComponent<Building_Ghost>().isSelected = false;
                    }

                    //Update ghostPointedAt
                    ghost_PointedAt = hitTransform.gameObject;
                    Building_Ghost selectedGhost = ghost_PointedAt.gameObject.GetComponent<Building_Ghost>();

                    //Show The Selected Ghost
                    if (!CheckOverlappingGhost(selectedGhost.gameObject.transform))
                    {
                        buildingBlockCanBePlaced = true;

                        switch (buildingType_Selected)
                        {
                            case BuildingType.None:
                                break;

                            case BuildingType.Floor:
                                if (selectedGhost.buildingType == BuildingType.Floor)
                                {
                                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = canPlace_Material;
                                    selectedGhost.isSelected = true;
                                }
                                break;
                            case BuildingType.Wall:
                                if (selectedGhost.buildingType == BuildingType.Wall)
                                {
                                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = canPlace_Material;
                                    selectedGhost.isSelected = true;
                                }
                                break;
                            case BuildingType.Wall_Diagonaly:
                                if (selectedGhost.buildingType == BuildingType.Wall_Diagonaly)
                                {
                                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = canPlace_Material;
                                    selectedGhost.isSelected = true;
                                }
                                break;
                            case BuildingType.Stair:
                                if (selectedGhost.buildingType == BuildingType.Stair)
                                {
                                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = canPlace_Material;
                                    selectedGhost.isSelected = true;
                                }
                                break;
                            case BuildingType.Angeled:
                                if (selectedGhost.buildingType == BuildingType.Angeled)
                                {
                                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = canPlace_Material;
                                    selectedGhost.isSelected = true;
                                }
                                break;
                            case BuildingType.Angeled_Corner:
                                if (selectedGhost.buildingType == BuildingType.Angeled_Corner)
                                {
                                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = canPlace_Material;
                                    selectedGhost.isSelected = true;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    if (ghost_PointedAt != null)
                    {
                        ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = invisible_Material;
                        ghost_PointedAt.gameObject.GetComponent<Building_Ghost>().isSelected = false;
                        buildingBlockCanBePlaced = false;
                    }

                    //Update ghostPointedAt
                    ghost_PointedAt = null;
                }
            }
        }
        else
        {
            if (ghost_PointedAt != null)
            {
                ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = invisible_Material;
                ghost_PointedAt.gameObject.GetComponent<Building_Ghost>().isSelected = false;
                buildingBlockCanBePlaced = false;
            }

            ghost_PointedAt = null;

            SetGhostInactive();
        }
    }
    void SetActiveGhost(BuildingType buildingType)
    {
        print("1. SetActiveGhost");

        BuildingBlock_Parent selectedBuildingBlock = buildingBlock_PointedAt.GetComponent<BuildingBlock>().buidingBlock_Parent.GetComponent<BuildingBlock_Parent>();

        for (int i = 0; i < selectedBuildingBlock.ghostList.Count; i++)
        {
            if (selectedBuildingBlock.ghostList[i].GetComponent<Building_Ghost>().buildingType == buildingType
                && !selectedBuildingBlock.ghostList[i].GetComponent<Building_Ghost>().isDeleted)
            {
                print("2. SetActiveGhost");
                selectedBuildingBlock.ghostList[i].SetActive(true);
            }
            else
            {
                print("3. SetActiveGhost");
                selectedBuildingBlock.ghostList[i].SetActive(false);
            }
        }
    }
    void SetGhostInactive()
    {
        if (lastBuildingBlock_PointedAt != null)
        {
            if (buildingBlock_PointedAt == null && ghost_PointedAt == null)
            {
                BuildingBlock_Parent selectedBuildingBlock = lastBuildingBlock_PointedAt.GetComponent<BuildingBlock>().buidingBlock_Parent.GetComponent<BuildingBlock_Parent>();

                for (int i = 0; i < selectedBuildingBlock.ghostList.Count; i++)
                {
                    selectedBuildingBlock.ghostList[i].SetActive(false);
                }
            }
        }
    }

    bool CheckOverlappingGhost(Transform BuildingBlockSuggestedPosition)
    {
        //Check if BuildingBlockTransform has the same position as any other buldingblock
        for (int i = 0; i < buildingBlockList.Count; i++)
        {
            if (BuildingBlockSuggestedPosition.position == buildingBlockList[i].transform.position)
            {
                return true;
            }
        }

        return false;
    }


    //--------------------


    public void PlaceBlock()
    {
        if (ghost_PointedAt != null && !ghost_PointedAt.GetComponent<Building_Ghost>().isDeleted && buildingBlockCanBePlaced)
        {
            print("1. Block is placed");

            //Floor
            if (ghost_PointedAt.GetComponent<Building_Ghost>().buildingType == BuildingType.Floor)
            {
                //Wood
                if (buildingMaterial_Selected == BuildingMaterial.Wood)
                {
                    print("Placed: Wood");

                    buildingBlockList.Add(Instantiate(builingBlock_Floor) as GameObject);
                    buildingBlockList[buildingBlockList.Count - 1].transform.SetParent(buildingBlock_Parent.transform);
                    buildingBlockList[buildingBlockList.Count - 1].transform.position = ghost_PointedAt.transform.position;

                    //ghost_PointedAt.GetComponent<Building_Ghost>().isDeleted = true;

                    switch (ghost_PointedAt.GetComponent<Building_Ghost>().blockDirection)
                    {
                        case BlockDirection.None:
                            break;

                        case BlockDirection.North:
                            for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                            {
                                if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.South)
                                {
                                    //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                                }
                            }
                            break;
                        case BlockDirection.East:
                            for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                            {
                                if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.West)
                                {
                                    //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                                }
                            }
                            break;
                        case BlockDirection.South:
                            for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                            {
                                if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.North)
                                {
                                    //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                                }
                            }
                            break;
                        case BlockDirection.West:
                            for (int i = 0; i < buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList.Count; i++)
                            {
                                if (buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().blockDirection == BlockDirection.East)
                                {
                                    //buildingBlockList[buildingBlockList.Count - 1].GetComponent<BuildingBlock_Parent>().ghostList[i].GetComponent<Building_Ghost>().isDeleted = true;
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    //ghost_PointedAt.GetComponent<Building_Ghost>().isDeleted = true;
                }

                //Iron
                else if (buildingMaterial_Selected == BuildingMaterial.Iron)
                {
                    print("Placed: Iron");

                }

                //Indium
                else if (buildingMaterial_Selected == BuildingMaterial.Indium)
                {
                    print("Placed: Indium");

                }
            }
            

        }
    }
}
