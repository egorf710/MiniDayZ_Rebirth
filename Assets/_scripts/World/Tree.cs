using Assets._scripts.Interfaces;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : NetworkBehaviour, Interactable, Statetable
{
    [SerializeField] private int strength;
    [SerializeField] private AudioClip[] treeFallClip;
    [SerializeField] private ItemLoot[] itemLoot;
    [SerializeField] private GameObject deathTreeObject;
    [SerializeField] private SpriteRenderer mySpriteRenderer;
    [SerializeField] private Transform myparent;
    [SerializeField] private int dropCount = 3;
    [SerializeField] private int state;
    public void Interact()
    {
        if(strength <= 0) { return; }

        if (isServer)
        {
            strength--;

            if (strength <= 0)
            {


                ItemLootData[] itemToDrop = ItemRandomizer.GetItems(itemLoot, dropCount, true);

                foreach (var loot in itemToDrop)
                {
                    Item item = Resources.Load<Item>("Items/" + loot.itemPath);
                    InventoryManager.InstantiateItem(transform.position, item, loot.amount);
                }

                deathTreeObject.SetActive(true);
                mySpriteRenderer.color = new Color(1, 1, 1, 0);

                NetworkServer.SendToAll(new SetStateMessage { object_netID = netId, state = 1 });
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(treeFallClip[Random.Range(0, treeFallClip.Length)], transform.position);
            NetworkClient.Send(new InteractMessage { object_netID = netId });
        }
        //нет проверки на читерство!!! игрок может спавнить предметы
    }


    public bool IsInteractable(out string message)
    {
        if (isServer)
        {
            //Изменить ибо сервер не проверяет есть ли у игрока топор и т.д и это может быть читерство!
            bool can = strength > 0;
            message = (can ? "(OK) игрок может срубить дерево: " : "(НЕТ) игрок не может срубить дерево: ") + gameObject.name;
            return can;
        }
        else
        {
            bool can = InventoryManager.PlayerHasAxeInHand() && strength > 0;

            message = (can ? "(OK) игрок может срубить дерево: " : "(НЕТ) игрок не может срубить дерево: ") + gameObject.name;

            return can;
        }
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
        gameObject.name += netId;
        SetStatetable(state);
    }

    public void SetStatetable(int state)
    {
        this.state = state;
        if (state == 0)
        {
            strength = 3;
            deathTreeObject.SetActive(false);

            mySpriteRenderer.color = new Color(1, 1, 1, 1);

        }
        else if(state == 1)
        {
            strength = 0;
            deathTreeObject.SetActive(true);

            mySpriteRenderer.color = new Color(1, 1, 1, 0);

        }
        if(isServer)
        {
            NetworkServer.SendToAll(new SetStateMessage { object_netID = netId, state = this.state });
        }
    }

    public Vector2 GetPosition()
    {
        return myparent.transform.position;
    }

    public bool IsActive()
    {
        return InventoryManager.PlayerHasAxeInHand() && strength > 0;
    }

    public int GetStatetable()
    {
        return state;
    }
}
