using Assets._scripts;
using Assets._scripts.Menu;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ItemObject;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemObjectPrefab;
    [SerializeField] public static InventoryManager Instance;
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] public Transform player;
    [SerializeField] private ItemMenu itemMenu;
    [SerializeField] private Transform outPanel;
    [SerializeField] private Image dropPanel;
    [SerializeField] private Transform dropitemsParent;
    [SerializeField] private GameObject dropItemSlotPrefab;
    [SerializeField] public bool InventoryOpen;
    public void Init(Transform player)
    {
        Instance = this;
        this.player = player;

        foreach (var slot in slots)
        {
            slot.Init();
            if(slot.slotType == ItemType.pants || slot.slotType == ItemType.head || slot.slotType == ItemType.body || slot.slotType == ItemType.backpack || slot.slotType == ItemType.shield)
            {
                clothesSlots.ToList().Add(slot);
            }
        }
        SetActiveInventory(true);
        SetActiveInventory(false);
    }
    public static bool AddItem(ItemInfo itemInfo)
    {

        var notFullCollections = Instance.slots
            .Where(state =>(!state.IsEmpty
            && (itemInfo.item == state.itemInfo.item)
            && (state.itemInfo.amount < state.itemInfo.item.item_max_amount)) || (state.IsEmpty && !state.IsLocked && itemInfo.item.item_type == state.slotType)
            || (state.IsEmpty && state.slotType == ItemType.def && itemInfo.item.item_type == ItemType.food)).ToList();
        
        foreach (var slot in notFullCollections)
        {
            if (slot.IsLocked) { continue; }
            if (!slot.IsEmpty)
            {
                if (slot.itemInfo.item.stacable)
                {
                    int diff = slot.itemInfo.item.item_max_amount - slot.itemInfo.amount - itemInfo.amount;

                    if (diff < 0)
                    {
                        if (!OptionsData.autoDistributeItem)
                        {
                            return false;
                        }
                        else
                        {
                            slot.SetSlot(itemInfo);
                            itemInfo.amount = Mathf.Abs(diff);
                        }
                    }
                    else
                    {
                        slot.SetSlot(itemInfo);
                        itemInfo.amount = Mathf.Abs(diff);
                        //print("add " + itemInfo.item.item_name);
                        return true;
                    }
                }
            }
            else
            {
                if (itemInfo.item.stacable)
                {
                    int diff = itemInfo.item.item_max_amount - itemInfo.amount;
                    if (diff < 0)
                    {
                        slot.SetSlot(itemInfo);
                        itemInfo.amount = Mathf.Abs(diff);
                        if (!OptionsData.autoDistributeItem)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        slot.SetSlot(itemInfo);
                        itemInfo.amount = Mathf.Abs(diff);
                        //print("add " + itemInfo.item.item_name);
                        return true;
                    }
                }
                else
                {
                    slot.SetSlot(itemInfo);
                    //print("add " + itemInfo.item.item_name);
                    return true;
                }
            }
        }
        return false;
    }
    public static void Drop(InventorySlot inventorySlot)
    {
        ItemObject itemObject = Instantiate(Instance.itemObjectPrefab, Instance.player.position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(inventorySlot);
    }
    public static void DropAllInventory()
    {
        foreach (var slot in Instance.slots)
        {
            if (slot.itemInfo != null && slot.itemInfo.item != null && !slot.IsEmpty)
            {
                slot.DropSlot();
            }
        }
    }
    public static void InstantiateItem(Item item, int amount = 1, int durability = 100, int ammo = 0)
    {
        ItemInfo itemInfo = new ItemInfo
        {
            name = item.name,
            item = item,
            amount = amount,
            durability = durability,
            ammo = ammo
        };
    }
    public static void InstantiateItem(Vector3 pos, Item item, int amount = 1, int durability = 100, int ammo = 0)
    {
        ItemInfo itemInfo = new ItemInfo
        {
            name = item.name,
            item = item,
            amount = amount,
            durability = durability,
            ammo = ammo
        };
    }
    public static void InstantiateItem(ItemInfo itemInfo)
    {
        ItemObject itemObject = Instantiate(Instance.itemObjectPrefab, Instance.player.position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(itemInfo);
    }
    public static Item GetItemByName(string name)
    {
        //print(name);
        return Resources.Load<Item>(name);
    }
    public void OpenClose()
    {
        SetActiveInventory(!transform.GetChild(0).gameObject.activeSelf);
        InventoryOpen = transform.GetChild(0).gameObject.activeSelf;
        Instance.ClearTemp();
    }
    public static void SetActiveInventory(bool b)
    {
        Instance.transform.GetChild(0).gameObject.SetActive(b);
        Instance.InventoryOpen = Instance.transform.GetChild(0).gameObject.activeSelf;
        Instance.ClearTemp();
    }
    public static void OpenItemMenu(InventorySlot inventorySlot, Vector2 point)
    {
        Instance.itemMenu.gameObject.SetActive(true);
        Instance.itemMenu.transform.position = point;
        Instance.itemMenu.Set(inventorySlot);
    }
    public static Transform GetOutTransform()
    {
        return Instance.outPanel;
    }
    public static void SetActiveDropPanel(bool b)
    {
        Instance.dropPanel.gameObject.SetActive(b);
    }
    public static void RefreshDropPanel()
    {
        Instance.InstanceRefreshDropPanel();
    }
    private void InstanceRefreshDropPanel()
    {
        if (!InventoryOpen) { return; }

        for (int i = 0; i < dropitemsParent.childCount; i++)
        {
            Destroy(dropitemsParent.GetChild(i).gameObject);
        }

        //ItemObject[] items = playerInteract.GetAroundItems();

        //foreach (var item in items)
        //{
        //    GameObject dropSlot = Instantiate(dropItemSlotPrefab, dropitemsParent);
        //    dropSlot.GetComponent<DropSlot>().SetSlot(item);
        //}
    }
    private void ClearTemp()
    {
        RefreshDropPanel();
    }

    //EVENT BUS
    public static void SetClothes(Item item, bool isWeapon = false)
    {
        if(item == null) { Debug.LogWarning("item is null!"); return; }
        Instance.player.GetChild(0).GetComponent<PlayerAnimator>().SetClothByName(item.name, isWeapon);
    }

    //EVENT BUS
    public static void ClearClothes(string itemName)
    {
        Instance.player.GetChild(0).GetComponent<PlayerAnimator>().ClearClothByName(itemName);
    }
    public static void ClearClothes(ItemType itemType)
    {
        Instance.player.GetChild(0).GetComponent<PlayerAnimator>().ClearClothByType(itemType);
    }
    public static void ClearClothes(AnimationThpe animType)
    {
        Instance.player.GetChild(0).GetComponent<PlayerAnimator>().ClearClothByType(animType);
    }


    /// <summary>
    /// Switch to pistol when pislot and rifle == null
    /// Switch to rifle when pistol != null or pistol == null
    /// </summary>
    public static void SwitchToNewWeapon(bool mainSwitch = false)
    {
    }
    public static InventorySlot GetSlotByItem(Item item)
    {
        if (Instance.slots.Count == 0) { return null; }

        var list = Instance.slots.Where(x => !x.SlotIsNull() && x.itemInfo.item == item).ToArray();

        return (list.Length > 0 ? list.First() : null);
    }

    [ShowInInspector] public InventorySlot[] clothesSlots = new InventorySlot[5];

    public static void ClothesDamage(int damage = 1)
    {
        Instance.ClothesDamageInstance(damage);
    }
    private void ClothesDamageInstance(int damage)
    {
        foreach (var slot in clothesSlots)
        {
            if (slot != null && !slot.IsEmpty)
            {
                slot.itemInfo.durability -= damage;
            }
        }
    }
}
