using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
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
    private Item usesItem;
    private void Start()
    {
        weaponManager = FindAnyObjectByType<WeaponManager>();
        playerNetwork = weaponManager.weaponController.GetComponent<PlayerNetwork>();//TODO
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
    }
    public void Interact()
    {
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
                if(usesSlot.itemInfo.amount <= 0)
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
    public void Drop()
    {
        usesSlot.DropSlot();
        gameObject.SetActive(false);
    }
    public void ShowItemInfo()
    {
        itemInfoPanel.SetActive(true);
    }
}
