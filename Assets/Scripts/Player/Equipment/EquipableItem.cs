using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    public Animator animator;
    public ItemSubCategories subCategories;


    //--------------------


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    //--------------------


    public void HitAnimation()
    {
        GameObject selectedTree = SelectionManager.instance.selectedTree;

        if (selectedTree != null)
        {
            selectedTree.GetComponent<ChoppableTree>().GetHit();
        }

        animator.SetTrigger("hit");
    }

    public void GetHit()
    {
        //The point in the animation where equipped item hits

        //If Axe is equipped
        if (subCategories == ItemSubCategories.Axe && SelectionManager.instance.selectedTree != null)
        {
            SelectionManager.instance.selectedTree.GetComponent<ChoppableTree>().treeParent.gameObject.GetComponent<TreeParent>().ObjectInteraction();
        }
    }

    public void RemoveDurability()
    {
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

            EquipmentManager.instance.DisplayEquippedModel(HandManager.instance.selectedSlotItem);
        }
    }
}
