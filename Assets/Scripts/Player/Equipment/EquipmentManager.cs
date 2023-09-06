using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance { get; private set; } //Singleton

    public bool itemIsbeingEquipped;

    [Header("Parent")]
    public GameObject toolHolderParent;


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

        if (item.equippedPrefab != null)
        {
            Instantiate(item.equippedPrefab, toolHolderParent.transform);
        }
    }

    void ActivateEquippedItem()
    {
        if (itemIsbeingEquipped)
        {
            itemIsbeingEquipped = false;

            return;
        }

        print(HandManager.instance.selectedSlotItem.itemName + " is equipped. | isEquipeable: " + HandManager.instance.selectedSlotItem.isEquipable);

        //Check if item has required states
        if (HandManager.instance.selectedSlotItem.isEquipable && toolHolderParent.GetComponentInChildren<EquipableItem>() != null)
        {
            //Play animation
            toolHolderParent.GetComponentInChildren<EquipableItem>().HitAnimation();
        }
    }
}
