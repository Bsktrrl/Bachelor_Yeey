using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObjectsManager : MonoBehaviour
{
    public static PickUpObjectsManager instance { get; private set; } //Singleton

    public List<GameObject> pickupsParents = new List<GameObject>();
    public List<PickupObjectData> pickupObject_CheckList = new List<PickupObjectData>();


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


    public void UpdatePickupObject_CheckList(Items itemname, int id)
    {
        for (int i = 0; i < pickupObject_CheckList.Count; i++)
        {
            if (pickupObject_CheckList[i].itemName == itemname && pickupObject_CheckList[i].id == id)
            {
                pickupObject_CheckList[i].isActive = true;

                Save();

                return;
            }
        }
    }
    public void UpdatePickupDataList()
    {
        //Get both .Count
        int tempSaveCount = pickupObject_CheckList.Count;
        int tempChildCount = 0;

        for (int j = 0; j < pickupsParents.Count; j++)
        {
            tempChildCount += pickupsParents[j].transform.childCount;
        }

        //Check if any new pickups have been added
        if (tempSaveCount < tempChildCount)
        {
            print("tempSaveCount " + tempSaveCount + " < tempChildCount " + tempChildCount);

            print("Pickups Added");

            //Check if list = 0
            if (pickupObject_CheckList.Count <= 0)
            {
                print("Pickups = 0");

                //add all avaliable data to list
                PickupObjectData tempData = new PickupObjectData();

                for (int j = 0; j < pickupsParents.Count; j++)
                {
                    for (int k = 0; k < pickupsParents[j].transform.childCount; k++)
                    {
                        pickupObject_CheckList.Add(tempData);

                        print("1. Itemname: j: " + j + " | k: " + k + " " + pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().itemName + " | ID: " + pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().id);

                        pickupObject_CheckList[pickupObject_CheckList.Count - 1].isActive = pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().isActive;
                        pickupObject_CheckList[pickupObject_CheckList.Count - 1].id = pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().id;
                        pickupObject_CheckList[pickupObject_CheckList.Count - 1].itemName = pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().itemName;
                        pickupObject_CheckList[pickupObject_CheckList.Count - 1].amount = pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().amount;

                        print("2. Itemname: " + pickupObject_CheckList[pickupObject_CheckList.Count - 1].itemName + " | ID: " + pickupObject_CheckList[pickupObject_CheckList.Count - 1].id);
                    }
                }

                for (int i = 0; i < pickupObject_CheckList.Count; i++)
                {
                    print("3. ItemName: " + pickupObject_CheckList[i].itemName + " | ID: " + pickupObject_CheckList[i].id);
                }
            }
            else
            {
                print("Pickups != 0");
            }
        }
        else
        {
            //Good to go
            print("No pickups Added");
        }

        //Destroy all active items
        for (int i = 0; i < pickupObject_CheckList.Count; i++)
        {
            //Find element that isActive = true
            if (pickupObject_CheckList[i].isActive)
            {
                print("Object is Active: " + i);

                //Destroy Object with the same ItemName and ID as this
                for (int j = 0; j < pickupsParents.Count; j++)
                {
                    for (int k = 0; k < pickupsParents[j].transform.childCount; k++)
                    {
                        if (pickupObject_CheckList[i].itemName == pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().itemName
                            && pickupObject_CheckList[i].id == pickupsParents[j].transform.GetChild(k).GetComponent<PickupObject>().id)
                        {

                            j = pickupsParents.Count;
                            k = pickupsParents[j].transform.childCount;

                            print("Destroy Object: " + k);

                            Destroy(pickupsParents[j].transform.GetChild(k));
                        }
                    }
                }
            }
        }

        Save();
    }

    void Load()
    {
        pickupObject_CheckList = DataManager.instance.pickupObject_CheckStoreList;

        UpdatePickupDataList();
    }
    void Save()
    {
        DataManager.instance.pickupObject_CheckStoreList = pickupObject_CheckList;
    }
}

[Serializable]
public class PickupObjectData
{
    public bool isActive;
    public double id;

    public Items itemName;
    public int amount;
}
