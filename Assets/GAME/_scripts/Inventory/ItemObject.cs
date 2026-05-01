using Assets.GAME._scripts.Fic;
using Assets.GAME._scripts.Inventory;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemObject : MonoBehaviour
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

    public void Set(ItemInfo itemInfo)
    {
        this.itemInfo = new ItemInfo(itemInfo);

        if (spriteRender != null)
        {
            spriteRender.sprite = itemInfo.item.item_drop_sprite;
        }
    }
}
