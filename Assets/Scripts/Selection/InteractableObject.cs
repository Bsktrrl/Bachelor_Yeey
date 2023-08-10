using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] [TextArea (5, 10)] string ItemName;

    public string GetItemName()
    {
        return ItemName;
    }
}