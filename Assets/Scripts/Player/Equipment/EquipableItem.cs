using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    public Animator animator;


    //--------------------


    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    //--------------------


    public void HitAnimation()
    {
        animator.SetTrigger("hit");
    }
}