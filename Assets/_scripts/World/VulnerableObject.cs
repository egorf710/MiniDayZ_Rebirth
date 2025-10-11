using Assets._scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulnerableObject : MonoBehaviour
{
    [SerializeField] public ItemLoot[] itemLoots;
    [SerializeField] private MonoBehaviour controller;
    private void Awake()
    {
        if(!(controller is Vulnerable))
        {
            Debug.LogError(controller + " controller is not Vulnerable");
        }
    }

    public void TakeDamage(int damage)
    {  
        (controller as Vulnerable).TakeDamage(damage);
    }
    public void DropLoot()
    {
        ItemLootData[] itemToDrop = ItemRandomizer.GetItems(itemLoots, 1, true);
        ServerManager.instance.DropLoot(new DropLootMassage()
        {
            loot = itemToDrop,
            pos = transform.position
        });
    }
}
