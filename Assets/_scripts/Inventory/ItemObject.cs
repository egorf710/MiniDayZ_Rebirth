using Assets._scripts.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemObject : MonoBehaviour, Interactable
{
    [SerializeField] private SpriteRenderer spriteRender;
    public ItemInfo itemInfo;

    private void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        if (itemInfo != null && itemInfo.item)
        {
            spriteRender.sprite = itemInfo.item.item_drop_sprite;
        }
    }
    public void Set(Item item, int amount = 1, int durability = 100, int ammo = 0)
    {
        itemInfo = new ItemInfo
        {
            name = item.name,
            item = item,
            amount = amount,
            durability = durability,
            ammo = ammo
        };
        if (spriteRender != null)
        {
            spriteRender.sprite = item.item_drop_sprite;
        }
    }
    public void Set(ItemInfo itemInfo)
    {
        itemInfo = new ItemInfo
        {
            name = itemInfo.item.name,
            item = itemInfo.item,
            amount = itemInfo.amount,
            durability = itemInfo.durability,
            ammo = itemInfo.ammo
        };
        if (spriteRender != null)
        {
            spriteRender.sprite = itemInfo.item.item_drop_sprite;
        }
    }
    public void Set(InventorySlot slot)
    {
        itemInfo = new ItemInfo
        {
            name = slot.itemInfo.item.name,
            item = slot.itemInfo.item,
            amount = slot.itemInfo.amount,
            durability = slot.itemInfo.durability,
            ammo = slot.itemInfo.ammo
        };
        if (spriteRender != null)
        {
            spriteRender.sprite = slot.itemInfo.item.item_drop_sprite;
        }
        itemInfo.insideItems = new List<ItemInfo>();
        foreach (var item in slot.insideSlots)
        {
            itemInfo.insideItems.Add(Inverse(item));
        }
    }
    private ItemInfo Inverse(InventorySlot slot)
    {
        if(slot.itemInfo == null || slot.itemInfo.item == null) { return null; }
        ItemInfo itemInfo = new ItemInfo
        {
            name = slot.itemInfo.item.name,
            item = slot.itemInfo.item,
            amount = slot.itemInfo.amount,
            durability = slot.itemInfo.durability,
            ammo = slot.itemInfo.ammo
        };
        foreach (var item in slot.insideSlots)
        {
            itemInfo.insideItems.Add(new ItemInfo()
            {
                name = item.itemInfo.item.name,
                item = item.itemInfo.item,
                amount = item.itemInfo.amount,
                durability = item.itemInfo.durability,
                ammo = item.itemInfo.ammo
            });
        }
        return itemInfo;
    }

    public void Interact()
    {
        if (InventoryManager.AddItem(itemInfo))
        {
            Destroy(gameObject);
        }
    }

    public bool IsInteractable(out string message)
    {
        float dist = Vector2.Distance(InventoryManager.Instance.player.position, transform.position);
        if (dist > 0.5f)
        {
            message = "Игрок слишком далеко чтобы подобрать предмет. Дистанция: " + dist;
            InventoryManager.Instance.player.GetComponent<PlayerMove>().GoToPoint(transform.position);
            return false;
        }
        else
        {
            message = "Игрок может подобрать предмет";
            return true;
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public bool IsActive()
    {
        return true;
    }

    [Serializable]
    public class ItemInfo
    {
        public string name;
        public Item item;
        public int amount = 0;
        public int durability = 0;
        public int ammo;
        public List<ItemInfo> insideItems;

        public ItemInfo(ItemInfo itemInfo)
        {
            this.name = itemInfo.name;
            this.item = itemInfo.item;
            this.amount = itemInfo.amount;
            this.durability = itemInfo.durability;
            this.ammo = itemInfo.ammo;
            insideItems = new List<ItemInfo>();
        }
        public ItemInfo()
        {
        }
    }
}
