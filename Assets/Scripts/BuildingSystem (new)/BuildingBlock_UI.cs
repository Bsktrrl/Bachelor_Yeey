using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingBlock_UI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] BuildingType BuildingType;
    [SerializeField] BuildingMaterial BuildingMaterial;

    public void OnPointerEnter(PointerEventData eventData)
    {
        BuildingManager.instance.buildingType_Selected = BuildingType;
        BuildingManager.instance.buildingMaterial_Selected = BuildingMaterial;

        BuildingSystemMenu.instance.SetSelectedImage(gameObject.GetComponent<Image>().sprite);
    }
}
