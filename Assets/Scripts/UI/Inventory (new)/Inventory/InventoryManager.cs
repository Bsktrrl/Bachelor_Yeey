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
    }


    //--------------------


    public void AddInventory(int size)
    {
        Inventories _inventory = new Inventories();

        inventories.Add(_inventory);

        inventories[inventories.Count - 1].index = inventories.Count - 1;
        inventories[inventories.Count - 1].inventorySize = size;

        Save();

        print("Added inventory");
    }
    public void RemoveInventory(int index)
    {
        inventories.RemoveAt(index);

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
        DataManager.instance.inventories = inventories;
        DataPersistanceManager.instance.SaveGame();

        print("All data Saved");
    }
    void Load()
    {
        //DataPersistanceManager.instance.LoadGame();
        inventories = DataManager.instance.inventories;

        print("All data Loaded");
    }
}
