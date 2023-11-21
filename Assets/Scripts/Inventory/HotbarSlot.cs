using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image selectedImage;

    public int ID;
    public Items hotbarItem;


    //--------------------


    private void Awake()
    {
        selectedImage.gameObject.SetActive(false);
    }


    //--------------------


    public void SetHotbarSlotImage()
    {
        image.sprite = MainManager.instance.GetItem(hotbarItem).hotbarSprite;
    }
    public void RemoVeHotbarSlotImage()
    {
        image.sprite = MainManager.instance.GetItem(0).hotbarSprite;
    }

    public void ResetHotbarItem()
    {
        hotbarItem = Items.None;
    }

    public void SetHotbarSlotActive()
    {
        selectedImage.gameObject.SetActive(true);
    }
    public void SetHotbarSlotUnactive()
    {
        selectedImage.gameObject.SetActive(false);
    }
}
