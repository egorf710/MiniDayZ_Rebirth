using Assets._scripts;
using Assets._scripts.Menu;
using Assets.GAME._scripts.Fic;
using Assets.GAME._scripts.Inventory;
using Assets.GAME._scripts.Inventory.Signals;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ItemObject;

public class S_Inventory: MonoBehaviour
{
    [SerializeField] private GameObject itemObjectPrefab;
    [SerializeField] private InventorySlot[] clothesSlots = new InventorySlot[5];
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private ItemMenu itemMenu;
    private bool InventoryOpen;

    [Space]
    [Header("UI")]
    [SerializeField] private Transform outPanel;
    [SerializeField] private Image dropPanel;
    [SerializeField] private Transform dropitemsParent;
    [SerializeField] private GameObject dropItemSlotPrefab;


    public Item item;
    public ItemInfo[] insitem;
    public ItemObject[] itemsDrops;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddItem(item);
        }
    }
    public void Init()
    {
        foreach (var slot in slots)
        {
            slot.Init();
            if(slot.slotType == ItemType.pants || slot.slotType == ItemType.head || slot.slotType == ItemType.body || slot.slotType == ItemType.backpack || slot.slotType == ItemType.shield)
            {
                clothesSlots.ToList().Add(slot);
            }
        }
        EventBus.Subscribe<Signal_RefreshDropPanel>(RefreshDropPanel);
    }
    public bool AddItem(Item item)
    {
        ItemInfo itemInfo = new ItemInfo()
        {
            name = item.name,
            item = item,
            ammo = 0,
            amount = 1,
            durability = 100,
            insideItems = new List<ItemInfo>(insitem),
        };
        return AddItem(itemInfo);
    }
    public bool AddItem(ItemInfo itemInfo)
    {

        var notFullCollections = slots
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
    public void DropAllInventory()
    {
        foreach (var slot in slots)
        {
            if (slot.itemInfo != null && slot.itemInfo.item != null && !slot.IsEmpty)
            {
                slot.DropSlot();
            }
        }
    }

    public void InstantiateItem(ItemInfo itemInfo)
    {
        ItemObject itemObject = Instantiate(itemObjectPrefab, ServiceLocator.Get<GamePlayManager>().GetMyPlayer().transform.position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(itemInfo);

        EventBus.Invoke<Signal_UpdateDropItems>(new Signal_UpdateDropItems(InventoryOpen));
    }
    public Item GetItemByName(string name)
    {
        return Resources.Load<Item>(name);
    }
    public void OpenClose()
    {
        SetActiveInventory(!transform.GetChild(0).gameObject.activeSelf);
        InventoryOpen = transform.GetChild(0).gameObject.activeSelf;
        EventBus.Invoke<Signal_UpdateDropItems>(new Signal_UpdateDropItems(InventoryOpen));
    }
    public void SetActiveInventory(bool b)
    {
        transform.GetChild(0).gameObject.SetActive(b);
        InventoryOpen = b;
        EventBus.Invoke<Signal_UpdateDropItems>(new Signal_UpdateDropItems(InventoryOpen));
    }
    public void OpenItemMenu(InventorySlot inventorySlot, Vector2 point)
    {
        itemMenu.gameObject.SetActive(true);
        itemMenu.transform.position = point;
        itemMenu.Set(inventorySlot);
    }
    public void SetActiveDropPanel(bool b)
    {
        dropPanel.gameObject.SetActive(b);
    }
    public Transform GetOutTransform()
    {
        return outPanel;
    }

    private void RefreshDropPanel(Signal_RefreshDropPanel signal)
    {
        if (!InventoryOpen) { return; }

        for (int i = 0; i < signal.dropItems.Length; i++)
        {
            if (dropitemsParent.childCount > i)
            {
                dropitemsParent.GetChild(i).TryGetComponent<DropSlot>(out DropSlot drop);
                if (drop != null)
                {
                    if (!signal.dropItems.Contains(drop.myObject))
                    {
                        Destroy(dropitemsParent.GetChild(i).gameObject);
                    }
                    else
                    {
                        drop.SetSlot(signal.dropItems[i]);
                    }
                }
            }
            else
            {
                GameObject dropSlot = Instantiate(dropItemSlotPrefab, dropitemsParent);
                dropSlot.GetComponent<DropSlot>().SetSlot(signal.dropItems[i]);
            }
        }


    }

    public void SetClothes(Item item, bool isWeapon = false)
    {
        if(item == null) { Debug.LogWarning("item is null!"); return; }
        EventBus.Invoke<Signal_SetClothes>(new Signal_SetClothes(item, isWeapon));
    }

    //EVENT BUS
    public void ClearClothes(string itemName)
    {
        EventBus.Invoke<Signal_ClearClothes>(new Signal_ClearClothes(itemName));
    }
    public void ClearClothes(ItemType itemType)
    {
        EventBus.Invoke<Signal_ClearClothes>(new Signal_ClearClothes(itemType));
    }
    public void ClearClothes(AnimationThpe animType)
    {
        EventBus.Invoke<Signal_ClearClothes>(new Signal_ClearClothes(animType));
    }

/* InventoryManager (singleton)
    только:
        хранение слотов
        Add/Remove/Move/Split

InventoryUI
    рисует UI

EquipmentSystem
    экипировка

DropSystem
    выброс в мир

NetworkInventoryBridge
    Cmd/Rpc Mirror

EventBus
    склейка всего */


    public InventorySlot GetSlotByItem(Item item)
    {
        if (slots.Count == 0) { return null; }

        var list = slots.Where(x => !x.SlotIsNull() && x.itemInfo.item == item).ToArray();

        return (list.Length > 0 ? list.First() : null);
    }

    public void ClothesDamage(int damage)
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
