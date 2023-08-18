using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; } //Singleton

    public List<Inventories> inventories = new List<Inventories>();


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
        DataManager.datahasLoaded += Load;
        DataManager.dataIsSaving += Save;
        PlayerButtonManager.S_isClicked += Save;
    }


    //--------------------


    public void AddInventory(InventoryObject obj, int size)
    {
        Inventories _inventory = new Inventories();

        inventories.Add(_inventory);

        inventories[inventories.Count - 1].index = inventories.Count - 1;
        inventories[inventories.Count - 1].inventorySize = size;

        obj.inventoryIndex = inventories.Count - 1;

        Save();

        print("Added inventory");
    }
    public void RemoveInventory(int index)
    {
        inventories.RemoveAt(index);

        for (int i = index; i < inventories.Count; i++)
        {
            inventories[i].index -= 1;
        }

        print("Inventory removed");

        Save();
    }


    //--------------------


    public List<Inventories> FindOpenInventories()
    {
        List<Inventories> temp = new List<Inventories>();

        for (int i = 0; i < inventories.Count; i++)
        {
            if (inventories[i].isOpen)
            {
                temp.Add(inventories[i]);
            }   
        }

        return temp;
    }


    //--------------------


    void Save()
    {
        DataManager.instance.inventories_StoreList = inventories;

        print("InventoryManager - All data Saved");
    }
    void Load()
    {
        //DataPersistanceManager.instance.LoadGame();
        inventories = DataManager.instance.inventories_StoreList;

        print("InventoryManager - All data Loaded");
    }


    //--------------------

}
