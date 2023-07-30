using Assets._scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, Interactable, Revivedable
{
    private int strength;
    [SerializeField] private AudioClip[] treeFallClip;
    [SerializeField] private ItemLoot[] itemLoot;
    [SerializeField] private GameObject deathTreeObject;
    [SerializeField] private SpriteRenderer mySpriteRenderer;
    [SerializeField] private Transform myparent;
    [SerializeField] private int dropCount = 3;
    public bool IsAlive = true;
    public void Interact()
    {
        if(!IsAlive) { return; }

        strength--;

        if(strength <= 0)
        {
            AudioSource.PlayClipAtPoint(treeFallClip[Random.Range(0, treeFallClip.Length)], transform.position);

            ItemLoot[] itemToDrop = ItemRandomizer.GetItems(itemLoot, dropCount, true);

            foreach (var item in itemToDrop)
            {
                InventoryManager.InstantiateItem(item.item, item.amount);
            }

            mySpriteRenderer.color = new Color(1, 1, 1, 0);
            deathTreeObject.SetActive(true);
            IsAlive = false;
        }

        InventoryManager.Instance.player.GetComponent<PlayerMove>().GoToPoint(myparent.transform.position);
    }

    public bool IsInteractable(out string message)
    {
        message = "(OK) игрок может срубить дерево: " + gameObject.name;
        return IsAlive && InventoryManager.PlayerHasAxeInHand();
    }

    private void Start()
    {
        if(transform.parent != null)
        {
            myparent = transform.parent;
        }
        else
        {
            myparent = transform;
        }

        Revived();
    }

    public void Revived()
    {
        strength = Random.Range(2, 3);
        deathTreeObject.SetActive(false);
        mySpriteRenderer.color = new Color(1, 1, 1, 1);
        IsAlive = true;
    }

    public Vector2 GetPosition()
    {
        return myparent.transform.position;
    }

    public bool IsActive()
    {
        return InventoryManager.PlayerHasAxeInHand() && IsAlive;
    }
}
