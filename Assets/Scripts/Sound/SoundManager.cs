using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; set; } //Singleton

    public AudioSource audioSource;

    //Inventory
    [SerializeField] AudioClip menu_Select_Clip;
    [SerializeField] AudioClip menu_DropItem_Clip;
    [SerializeField] AudioClip menu_SortInventory_Clip;

    //Crafting
    [SerializeField] AudioClip menu_ChangeCraftingScreen_Clip;
    [SerializeField] AudioClip menu_Crafting_Clip;
    [SerializeField] AudioClip menu_CannotCraft_Clip;



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


    //--------------------


    public void PlaySelect_Clip()
    {
        if (audioSource != null)
        {
            audioSource.clip = menu_Select_Clip;
            audioSource.volume = 0.50f;
            audioSource.Play();
        }
    }
    public void PlayChangeCraftingScreen_Clip()
    {
        if (audioSource != null)
        {
            audioSource.clip = menu_ChangeCraftingScreen_Clip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }
    public void PlayDropItem_Clip()
    {
        if (audioSource != null)
        {
            audioSource.clip = menu_DropItem_Clip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }
    public void Playmenu_SortInventory_Clip()
    {
        if (audioSource != null)
        {
            audioSource.clip = menu_SortInventory_Clip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }
    public void Playmenu_Crafting_Clip()
    {
        if (audioSource != null)
        {
            audioSource.clip = menu_Crafting_Clip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }
    public void Playmenu_CanntoCraft_Clip()
    {
        if (audioSource != null)
        {
            audioSource.clip = menu_CannotCraft_Clip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }
}
