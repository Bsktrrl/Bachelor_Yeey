using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeParent : MonoBehaviour
{
    [SerializeField] GameObject Healthytree;
    [SerializeField] GameObject choppedTree;

    public Animator animator;

    int HP = 5;


    //--------------------

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    //--------------------


    public void ObjectInteraction()
    {
        if (Healthytree.GetComponent<ChoppableTree>().playerInRange && Healthytree.GetComponent<ChoppableTree>().canBeChopped
            && SelectionManager.instance.selectedTree == gameObject.GetComponentInChildren<ChoppableTree>().gameObject
            && MainManager.instance.menuStates == MenuStates.None
            && HandManager.instance.selectedSlotItem.subCategoryName == ItemSubCategories.Axe)
        {
            HP -= 1;

            if (HP <= 0)
            {
                animator.SetTrigger("TreeCut");

                if (EquipmentManager.instance.toolHolderParent != null)
                {
                    EquipmentManager.instance.toolHolderParent.GetComponentInChildren<EquipableItem>().RemoveDurability();
                }
            }
            else
            {
                animator.SetTrigger("TreeShake");
            }
        }
    }

    public void TreeHitGround()
    {
        int temp = choppedTree.transform.childCount;

        choppedTree.SetActive(true);
        ActivateGravity();

        for (int i = 0; i < temp; i++)
        {
            choppedTree.transform.GetChild(0).parent = MainManager.instance.treeParent.transform;
        }
        
        Destroy(gameObject);
    }

    void ActivateGravity()
    {
        for (int i = 0; i < choppedTree.transform.childCount; i++)
        {
            choppedTree.transform.GetChild(i).GetComponent<Log>().ActivateLogGravity();
        }
    }

    public int GetHealth()
    {
        return HP;
    }

    public void SetHealth(int _hp)
    {
        HP = _hp;
    }
}