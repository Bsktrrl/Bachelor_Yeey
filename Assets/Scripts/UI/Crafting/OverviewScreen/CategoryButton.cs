using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    public ItemCategories categoryType;

    public void CategoryButton_OnClick()
    {
        CraftingManager.instance.activeCategory = categoryType;
        CraftingManager.instance.SetupSelectionScreen();

        CraftingManager.instance.craftingScreen.SetActive(false);
    }
}
