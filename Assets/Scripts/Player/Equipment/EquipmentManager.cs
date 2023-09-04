using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance { get; private set; } //Singleton

    [Header("Parent")]
    [SerializeField] GameObject toolHolderParent;

    [Header("Models")]
    [SerializeField] GameObject axeModel;
    [SerializeField] GameObject buildersHammerModel;


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
        PlayerButtonManager.leftMouse_isPressedDown += ActivateEquippedItem;
    }


    //--------------------


    public void DisplayEquippedModel(Item item)
    {
        for (int i = 0; i < toolHolderParent.transform.childCount; i++)
        {
            Destroy(toolHolderParent.transform.GetChild(i).gameObject);
        }

        switch (item.itemName)
        {
            case Items.None:
                break;

            case Items.Stone:
                break;
            case Items.Plank:
                break;
            case Items.Leaf:
                break;
            case Items.Axe:
                Instantiate(axeModel, toolHolderParent.transform);
                break;
            case Items.BuildingHammer:
                Instantiate(buildersHammerModel, toolHolderParent.transform);
                break;
            case Items.SmallChest:
                break;
            case Items.MediumChest:
                break;

            default:
                break;
        }
    }

    void ActivateEquippedItem()
    {
        //print(HandManager.instance.selectedSlotItem.itemName + " is equipped. | isEquipeable: " + HandManager.instance.selectedSlotItem.isEquipable);

        ////Check if item has required states
        //if (HandManager.instance.selectedSlotItem.isEquipable)
        //{
        //    toolHolderParent.GetComponentInChildren<EquipableItem>().HitAnimation();

        //    HandManager.instance.selectedSlotItem.HP -= 1;
        //    StorageManager.instance.PlayerInventoryItemSlotList[HandManager.instance.selectedSlot].GetComponent<ItemSlot_N>().hp_Slider.value = HandManager.instance.selectedSlotItem.HP;

        //    //Delete item if HP <= 0
        //    if (HandManager.instance.selectedSlotItem.HP <= 0)
        //    {
        //        HandManager.instance.selectedSlotItem = StorageManager.instance.item_SO.itemList[0];
        //    }
        //}
    }
}
