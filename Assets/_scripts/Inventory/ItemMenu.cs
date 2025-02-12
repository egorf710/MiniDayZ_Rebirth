using Assets._scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour, Initable
{
    [SerializeField] private Button mainInteractButton;
    [SerializeField] private Button secondInteractButton;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemMenuTitle;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text mainInteractText;
    [SerializeField] private TMP_Text secondInteractText;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private GameObject unFocusedCloserPanel;
    private WeaponManager weaponManager;
    private PlayerNetwork playerNetwork;
    private InventorySlot usesSlot;
    private InventorySlot targetSlot;
    private Item usesItem;
    public static ItemMenu Instance;
    public void Init(Transform player)
    {
        Instance = this;
        weaponManager = FindAnyObjectByType<WeaponManager>();
        playerNetwork = player.GetComponent<PlayerNetwork>();//TODO
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        unFocusedCloserPanel.SetActive(true);
    }
    private void OnDisable()
    {
        unFocusedCloserPanel.SetActive(false);
    }
    public void Set(InventorySlot slot)
    {
        itemIcon.sprite = slot.itemInfo.item.item_sprite;
        itemDescription.text = slot.itemInfo.item.item_description;
        itemMenuTitle.text = slot.itemInfo.item.item_name;
        itemTitle.text = slot.itemInfo.item.item_name;
        usesSlot = slot;
        usesItem = slot.itemInfo.item;

        if(usesItem is ammoItem)
        {
            secondInteractButton.gameObject.SetActive(true);
            mainInteractText.text = "Зарядить в основное";
            secondInteractText.text = "Зарядить в дополнительное";
        }
        else
        {
            secondInteractButton.gameObject.SetActive(false);
        }
        if (usesItem is armorItem)
        {
            mainInteractButton.gameObject.SetActive(false);
        }
        else
        {
            mainInteractButton.gameObject.SetActive(true);
        }
        if (usesItem.item_type == ItemType.food)
        {
            foodItem fi = usesItem as foodItem;
            bool food = fi.food_point >= fi.water_point;

            if (usesItem.name.Contains("blueberries") || usesItem.name.Contains("cloudberry") || usesItem.name.Contains("Cranberry"))
            {
                food = true;
            }
            mainInteractText.text = food ? "Съесть" : "Выпить";
        }
        if(usesSlot.itemInfo.item.name == "duct tape")
        {
            if (targetSlot != null && !targetSlot.SlotIsNull())
            {
                mainInteractText.text = "Починить +15%";
                mainInteractButton.gameObject.SetActive(true);
                secondInteractButton.gameObject.SetActive(false);
            }
            else
            {
                mainInteractButton.gameObject.SetActive(false);
                secondInteractButton.gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
        }
    }
    public void Interact()
    {
        if(usesSlot == null || usesSlot.SlotIsNull()) { gameObject.SetActive(false); return; }
        if(usesItem is foodItem)
        {
            foodItem fi = usesItem as foodItem;

            if (!playerNetwork.playerAnimator.interact)
            {
                playerNetwork.playerAnimator.Interact(2f);

                var ph = playerNetwork.GetComponent<PlayerCharacteristics>();
                InventoryManager.SetActiveInventory(false);

                ph.playerFood += fi.food_point;
                ph.playerHealth += fi.heal_point;
                ph.playerWater += fi.water_point;
                ph.playerHeat += fi.temperature_point;

                usesSlot.itemInfo.amount--;
                if(usesSlot.itemInfo.amount <= 0)//TODO {set: get;}
                {
                    usesSlot.ClearSlot();
                    gameObject.SetActive(false);
                }
            }
        }
        else if(usesItem is ammoItem)
        {
            string debugMess;
            if (weaponManager.canReload(usesItem as ammoItem, true, out debugMess))
            {
                weaponManager.ReloadFor(usesSlot, 2);
            }
            else
            {
                DebuMessager.Mess(debugMess, Color.red);
            }
        }
        else if(usesSlot.itemInfo.item.name == "duct tape")
        {
            if (targetSlot == null || targetSlot.SlotIsNull()) { gameObject.SetActive(false); return; }
            if (targetSlot.itemInfo.item is armorItem && targetSlot.itemInfo.durability < 100)
            {
                targetSlot.itemInfo.durability += 15;
                usesSlot.itemInfo.amount--;
            }
            gameObject.SetActive(false);
            targetSlot.Refresh();
            usesSlot.Refresh();
        }
    }
    public void SecondInteract()
    {
        if (usesItem is ammoItem)
        {
            string debugMess;
            if (weaponManager.canReload(usesItem as ammoItem, false, out debugMess))
            {
                weaponManager.ReloadFor(usesSlot, 1);
                gameObject.SetActive(false);
            }
            else
            {
                DebuMessager.Mess(debugMess, Color.red);
            }
        }
    }
    public static bool CanSlotInteract(InventorySlot interactSlot, InventorySlot targetSlot)
    {
        if(interactSlot.itemInfo.item.name == "duct tape")
        {
            Instance.targetSlot = targetSlot;
            Instance.usesSlot = interactSlot;
            Instance.Set(interactSlot);
            return true;
        }
        return false;
    }
    public void Drop()
    {
        usesSlot.DropSlot();
        gameObject.SetActive(false);
    }
    public void ShowItemInfo()
    {
        itemInfoPanel.SetActive(true);
    }

    public void NetUpdate()
    {
        return;
    }
}
