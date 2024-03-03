using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemObject;

public class InventorySlot : MonoBehaviour
{
    public ItemInfo itemInfo;
    public List<InventorySlot> insideSlots;
    public ItemType slotType;
    public bool IsEmpty = true;
    public bool IsLocked = true;
    public bool IsSlotBlocked;
    [Header("Visual")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject itemBar;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text durabilityText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text heatText;

    public void Init()
    {
        if (itemInfo == null || (itemInfo != null && itemInfo.item == null))
        {
            ClearSlot();
        }
        itemIcon.gameObject.AddComponent<ItemMenuActivator>();
        itemIcon.GetComponent<ItemMenuActivator>().slot = this;
        itemIcon.TryGetComponent<SlotTaker>(out SlotTaker slotTaker);
        if(slotTaker != null)
        {
            slotTaker.Set(this);
        }
    }

    public void SetSlot(ItemInfo itemInfo)
    {
        if (this.itemInfo == null) { this.itemInfo = new ItemInfo(); }
        if(itemInfo == null || itemInfo.item == null) { ClearSlot(); return; }
        this.itemInfo.name = itemInfo.name;
        this.itemInfo.item = itemInfo.item;
        this.itemInfo.amount += itemInfo.amount;
        this.itemInfo.durability = itemInfo.durability;
        this.itemInfo.ammo = itemInfo.ammo;
        {
            if (this.itemInfo.amount > this.itemInfo.item.item_max_amount)
            {
                this.itemInfo.amount = this.itemInfo.item.item_max_amount;
            }
/*            if (this.itemInfo.durability > 100)
            {
                this.itemInfo.amount = 100;
            }*/
            if (this.itemInfo.durability < 0)
            {
                this.itemInfo.amount = 0;
            }
        }

        if (slotType == ItemType.backpack || slotType == ItemType.body || slotType == ItemType.pants || slotType == ItemType.head || slotType == ItemType.rifle || slotType == ItemType.pistol || slotType == ItemType.melee || slotType == ItemType.shield)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < this.itemInfo.item.slot_count; i++)
            {
                insideSlots[i].gameObject.SetActive(true);
                insideSlots[i].IsLocked = false;
            }
        }

        if (itemInfo.insideItems != null)
        {
            for (int i = 0; i < itemInfo.insideItems.Count; i++)
            {
                if (!insideSlots[i].IsLocked)
                {
                    insideSlots[i].SetSlot(itemInfo.insideItems[i]);
                }
            }
        }
        itemIcon.sprite = itemInfo.item.item_sprite;
        itemIcon.color = new Color(1, 1, 1, 1);

        if (this.itemInfo.item.stacable)
        {
            itemBar.SetActive(true);
            amountText.text = this.itemInfo.amount.ToString();
        }
        if (itemInfo.item is armorItem)
        {
            armorItem item = itemInfo.item as armorItem;
            if (item.heat > 0)
            {
                heatText.text += "Тепло +" + item.heat.ToString();
            }
            if (item.armor > 0)
            {
                heatText.text += "\nБроня +" + item.heat.ToString();
            }
        }
        if(itemInfo.item.item_type != ItemType.def && itemInfo.item.item_type != ItemType.food)
        {

        }
        if(itemInfo.item.item_type == ItemType.melee)
        {
            itemBar.SetActive(true);
            //durabilityText.text = ((this.itemInfo.item as weaponItem).durability / 100 * itemInfo.durability) + "%";
        }
        if(itemInfo.item.item_type == ItemType.rifle || itemInfo.item.item_type == ItemType.pistol)
        {
            itemBar.SetActive(true);
            //durabilityText.text = ((this.itemInfo.item as weaponItem).durability / 100 * itemInfo.durability) + "%";
            ammoText.text = this.itemInfo.ammo.ToString();
        }
        if (itemInfo.item is armorItem || itemInfo.item is weaponItem)
        {
            if (itemInfo.item is armorItem)
            {
                durabilityText.text = ((this.itemInfo.item as armorItem).durability / 100 * itemInfo.durability) + "%";
            }
            else if (itemInfo.item is weaponItem)
            {
                durabilityText.text = ((this.itemInfo.item as weaponItem).durability / 100 * itemInfo.durability) + "%";
            }


            InventoryManager.SetClothes(this.itemInfo.item, true);

            InventoryManager.SwitchToNewWeapon();
        }

        IsEmpty = false;
    }
    public void DropSlot()
    {
        if (IsSlotBlocked) { return; }
        InventoryManager.Drop(this);
        InventoryManager.SetActiveDropPanel(false);
        ClearSlot();
    }
    public void ClearSlot()
    {
        bool isWeapon = false;
        if (itemInfo != null && itemInfo.item != null)
        {
            isWeapon = itemInfo.item is armorItem || itemInfo.item is weaponItem;
            InventoryManager.ClearClothes(itemInfo.item.name);
        }
        else
        {
            InventoryManager.ClearClothes(slotType);
        }

        foreach (var slot in insideSlots)
        {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
            slot.IsLocked = true;
        }

        if (itemInfo != null)
        {

            itemInfo.item = null;
            itemInfo.amount = 0;
            itemInfo.durability = 0;
            itemInfo.ammo = 0;
            itemInfo = null;
        }
        itemIcon.color = new Color(1, 1, 1, 0);
        if (itemBar != null)
        {
            itemBar.SetActive(false);
        }
        if (amountText != null)
        {
            amountText.text = null;
        }
        if (durabilityText != null)
        {
            durabilityText.text = null;
        }
        if (ammoText != null)
        {
            ammoText.text = null;
        }
        if (heatText != null)
        {
            heatText.text = null;
        }
        if (slotType == ItemType.backpack || slotType == ItemType.body || slotType == ItemType.pants || slotType == ItemType.head || slotType == ItemType.rifle || slotType == ItemType.pistol || slotType == ItemType.melee || slotType == ItemType.shield)
        {
            gameObject.SetActive(false);
        }

        if (isWeapon)
        {
            InventoryManager.SwitchToNewWeapon();
        }

        IsEmpty = true;
    }

    public void Refresh()
    {
        if(this.itemInfo == null) { ClearSlot(); return; }
        {
            if (this.itemInfo.amount > this.itemInfo.item.item_max_amount)
            {
                this.itemInfo.amount = this.itemInfo.item.item_max_amount;
            }
            if (this.itemInfo.durability > 100)
            {
                this.itemInfo.amount = 100;
            }
            if (this.itemInfo.durability < 0)
            {
                this.itemInfo.amount = 0;
            }
        }

        if (slotType == ItemType.backpack || slotType == ItemType.body || slotType == ItemType.pants || slotType == ItemType.head || slotType == ItemType.rifle || slotType == ItemType.pistol || slotType == ItemType.melee || slotType == ItemType.shield)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < this.itemInfo.item.slot_count; i++)
            {
                insideSlots[i].gameObject.SetActive(true);
                insideSlots[i].IsLocked = false;
            }
        }

        itemIcon.sprite = itemInfo.item.item_sprite;
        itemIcon.color = new Color(1, 1, 1, 1);

        if (this.itemInfo.item.stacable)
        {
            itemBar.SetActive(true);
            amountText.text = this.itemInfo.amount.ToString();
        }
        if (itemInfo.item.item_type == ItemType.pants || itemInfo.item.item_type == ItemType.body || itemInfo.item.item_type == ItemType.head || itemInfo.item.item_type == ItemType.shield)
        {
            armorItem item = itemInfo.item as armorItem;
            if (item.heat > 0)
            {
                heatText.text += "Тепло +" + item.heat.ToString();
            }
            if (item.armor > 0)
            {
                heatText.text += "\nБроня +" + item.heat.ToString();
            }
        }
        if (itemInfo.item.item_type != ItemType.def && itemInfo.item.item_type != ItemType.food)
        {
            durabilityText.text = this.itemInfo.durability.ToString() + "%";
        }
        if (itemInfo.item.item_type == ItemType.rifle || itemInfo.item.item_type == ItemType.pistol)
        {
            itemBar.SetActive(true);
            durabilityText.text = this.itemInfo.durability.ToString() + "%";
            ammoText.text = this.itemInfo.ammo.ToString();
        }
    }
    /// <summary>
    /// TRUE IS NULL
    /// </summary>
    /// <returns></returns>
    
    public void RefreshAmmo()
    {
        //print("test");
        itemBar.SetActive(true);
        durabilityText.text = itemInfo.durability.ToString() + "%";
        ammoText.text = itemInfo.ammo.ToString();
    }
    public bool SlotIsNull()
    {
        if(itemInfo == null) { return true; }
        if(itemInfo.item == null) { return true; }
        return false;
    }
}
