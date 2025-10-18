using Assets._scripts.Interfaces;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : NetworkBehaviour, Interactable, Revivedable
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

            ItemLootData[] itemToDrop = ItemRandomizer.GetItems(itemLoot, dropCount, true);

            foreach (var loot in itemToDrop)
            {
                Item item = Resources.Load<Item>("Items/" + loot.itemPath);
                InventoryManager.InstantiateItem(item, loot.amount);
            }

            //mySpriteRenderer.color = new Color(1, 1, 1, 0);
            //deathTreeObject.SetActive(true);

            NetworkClient.Send(new SetActiveObjectMessage { object_netID = netId, state = false });

            //IsAlive = false;
        }

        //InventoryManager.Instance.player.GetComponent<PlayerMove>().GoToPoint(myparent.transform.position);
    }
    [Command(requiresAuthority = false)]
    private void CMDSetAlive(bool b)
    {
        CLTSetAlive(b);
    }
    [ClientRpc]
    private void CLTSetAlive(bool b)
    {
        mySpriteRenderer.color = new Color(1, 1, 1, b ? 1 : 0);
        deathTreeObject.SetActive(!b);
        IsAlive = b;
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
    }

    public void Revived()
    {
        strength = Random.Range(2, 3);
        //deathTreeObject.SetActive(false);

        NetworkClient.Send(new SetActiveObjectMessage { object_netID = netId, state = true });

        //mySpriteRenderer.color = new Color(1, 1, 1, 1);
        //IsAlive = true;
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
