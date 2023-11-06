using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    private Grid<GridObject> grid;

    int gridWidth = 6;
    int gridHeight = 8;
    float cellSize = 100f;

    //list of all items in inventory
    public List<Item> inventoryList = new List<Item>();

    public GameObject inventoryTab;
    public GameObject uiPrefab;
    public bool inventoryOpen;

    private void Awake()
    {
        Instance = this;

        GridObject.inventoryTab = inventoryTab;
        GridObject.uiPrefab = uiPrefab;

        //create the grid
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                inventoryTab.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                inventoryTab.SetActive(false);
            }
            inventoryOpen = !inventoryOpen;
        }
    }
}