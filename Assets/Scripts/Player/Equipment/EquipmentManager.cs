using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance { get; private set; } //Singleton

    public bool itemIsbeingEquipped;

    [Header("Parent")]
    [SerializeField] GameObject toolHolderParent;


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
            toolHolderParent.GetComponentInChildren<EquipableItem>().HitAnimation();

            StorageManager.instance.PlayerInventory.itemList[HandManager.instance.selectedSlot].hp -= 1;
            StorageManager.instance.PlayerInventoryItemSlotList[HandManager.instance.selectedSlot].GetComponent<ItemSlot_N>().hp_Slider.value = StorageManager.instance.PlayerInventory.itemList[HandManager.instance.selectedSlot].hp;

            //Delete item if HP <= 0
            if (StorageManager.instance.PlayerInventory.itemList[HandManager.instance.selectedSlot].hp <= 0)
            {
                print("Remove item");
                StorageManager.instance.RemoveItemFromInventory(HandManager.instance.selectedSlot);
                StorageManager.instance.PlayerInventoryItemSlotList[HandManager.instance.selectedSlot].GetComponent<ItemSlot_N>().hp_Slider.gameObject.SetActive(false);

                InventoryManager.instance.UpdateInventory(StorageManager.instance.PlayerInventory);

                StorageManager.instance.PlayerInventoryItemSlotList[HandManager.instance.selectedSlot].GetComponent<ItemSlot_N>().UpdateSlider();
                HandManager.instance.UpdateSlotInfo();

                DisplayEquippedModel(HandManager.instance.selectedSlotItem);
            }
        }
    }
}
