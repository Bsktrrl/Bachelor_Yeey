using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour
{
    [SerializeField] Button craftingButton;
    [SerializeField] Color craftingButtonColor_Inactive;
    [SerializeField] Color craftingButtonColor_Active;

    private void Update()
    {
        if (MainManager.instance.menuStates == MenuStates.InventoryMenu)
        {
            if (CraftingManager.instance.totalRequirementMet)
            {
                craftingButton.GetComponent<Image>().color = craftingButtonColor_Active;
            }
            else
            {
                craftingButton.GetComponent<Image>().color = craftingButtonColor_Inactive;
            }
        }
    }

    public void CraftingButton_OnClick()
    {
        if (CraftingManager.instance.totalRequirementMet)
        {
            //Remove items from inventory
            for (int i = 0; i < CraftingManager.instance.requirementPrefabList.Count; i++)
            {
                Items itemName = CraftingManager.instance.requirementPrefabList[i].GetComponent<CraftingRequirementPrefab>().requirements.itemName;
                int amount = CraftingManager.instance.requirementPrefabList[i].GetComponent<CraftingRequirementPrefab>().requirements.amount;

                for (int j = 0; j < amount; j++)
                {
                    //StorageManager.instance.RemoveLastItem(itemName);
                }
            }

            //StorageManager.instance.AddItem(CraftingManager.instance.itemSelected.itemName, 1);

            SoundManager.instance.Playmenu_Crafting_Clip();
        }
        else
        {
            SoundManager.instance.Playmenu_CanntoCraft_Clip();
        }
    }
}
