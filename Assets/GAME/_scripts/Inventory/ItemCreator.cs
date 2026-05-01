using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GAME._scripts.Inventory
{
    public class ItemCreator
    {
        public ItemInfo Create(Item item, int amount = 1, int durability = 100, int ammo = 0)
        {
            ItemInfo itemInfo = new ItemInfo
            {
                name = item.name,
                item = item,
                amount = amount,
                durability = durability,
                ammo = ammo
            };

            return itemInfo;
        }
        public ItemInfo SlotToItemInfo(InventorySlot slot)
        {
            return new ItemInfo(slot.itemInfo);
        }
    }
    [Serializable]
    public class ItemInfo
    {
        public string name;
        public Item item;
        public int amount = 0;
        public int durability = 0;
        public int ammo = 0;

        public List<ItemInfo> insideItems;

        public ItemInfo(ItemInfo itemInfo)
        {
            this.name = itemInfo.name;
            this.item = itemInfo.item;
            this.amount = itemInfo.amount;
            this.durability = itemInfo.durability;
            this.ammo = itemInfo.ammo;
            if (itemInfo.insideItems != null)
            {
                this.insideItems = new List<ItemInfo>(itemInfo.insideItems);
            }
        }

        public ItemInfo()
        {
        }
    }
}
