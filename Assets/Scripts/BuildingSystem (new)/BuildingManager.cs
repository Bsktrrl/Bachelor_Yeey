using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Ghost")]
    public GameObject lastBuildingBlock_PointedAt;
    public GameObject buildingBlock_PointedAt;
    public GameObject ghost_PointedAt;

    [Header("Selected")]
    public BuildingType buildingType_Selected = BuildingType.None;
    public BuildingMaterial buildingMaterial_Selected = BuildingMaterial.None;

    [Header("Materials")]
    [SerializeField] Material invisible_Material;
    [SerializeField] Material ghost_Material;
    [SerializeField] Material canPlace_Material;
    [SerializeField] Material cannotPlace_Material;

    //Wood
    [SerializeField] Material floor_Wood_Material;
    [SerializeField] Material wall_Wood_Material;
    [SerializeField] Material wall_Diagonaly_Wood_Material;
    [SerializeField] Material stair_Wood_Material;
    [SerializeField] Material angeled_Wood_Material;
    [SerializeField] Material angeled_Corner_Wood_Material;


    //--------------------


    private void Update()
    {
        RaycastGhost();
        SetGhostInactive();
    }


    //--------------------


    void RaycastGhost()
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

                //Update which Ghost to be active (able to select)
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
            else
            {
                if (ghost_PointedAt != null)
                {
                    ghost_PointedAt.gameObject.GetComponent<MeshRenderer>().material = invisible_Material;
                    ghost_PointedAt.gameObject.GetComponent<Building_Ghost>().isSelected = false;
                }

                //Update ghostPointedAt
                ghost_PointedAt = null;
            }
        }
    }

    void SetActiveGhost(BuildingType buildingType)
    {
        BuildingBlock_Parent selectedBuildingBlock = buildingBlock_PointedAt.GetComponent<BuildingBlock>().buidingBlock_Parent.GetComponent<BuildingBlock_Parent>();

        for (int i = 0; i < selectedBuildingBlock.ghostList.Count; i++)
        {
            if (selectedBuildingBlock.ghostList[i].GetComponent<Building_Ghost>().buildingType == buildingType)
            {
                selectedBuildingBlock.ghostList[i].SetActive(true);
            }
            else
            {
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
}
