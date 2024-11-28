using Assets._scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Image switchButtonImage;
    [SerializeField] private Image scopeButtonImage;
    [SerializeField] private Sprite[] switchSprites;
    [SerializeField] private Sprite[] scopeSprites;
    [SerializeField] private List<InventorySlot> weaponSlots;
    [SerializeField] private InventorySlot currentWeaponSlot;
    [SerializeField] private GameObject[] weaponUI;
    [SerializeField] public WeaponController weaponController;
    [SerializeField] private Animator ReloadImageAnimator;
    private bool reloading;
    //public void Init(params dynamic[] args)
    //{
    //    weaponController = args[0];
    //}
    public void Init(Transform player)
    {
        weaponController = player.GetComponent<WeaponController>();
        weaponController.aimOutline = FindObjectOfType<CanvasManager>().aimOutline;
    }
    public void SwitchWeapon()
    {
        //0 melee, 1 pistol, 2 rifle

        int index = weaponSlots.IndexOf(currentWeaponSlot);
        index++;
        if (index >= weaponSlots.Count) { index = 0; }

        currentWeaponSlot = weaponSlots[index];

        if (currentWeaponSlot.slotType != ItemType.melee && currentWeaponSlot.SlotIsNull()) { SwitchWeapon(); return; }

        switchButtonImage.sprite = switchSprites[index];

        if (!currentWeaponSlot.SlotIsNull())
        {
            InventoryManager.SetClothes(currentWeaponSlot.itemInfo.item, index != 0);

            weaponController.SetWeapon(currentWeaponSlot);
        }
        else
        {
            weaponController.SetWeapon(null);
            InventoryManager.ClearClothes(ItemType.melee);
        }

        RefreshUI(index);
    }

    public void SwitchWeapon(InventorySlot inventorySlot)
    {
        int index = weaponSlots.IndexOf(inventorySlot);

        currentWeaponSlot = weaponSlots[index];

        switchButtonImage.sprite = switchSprites[index];

        InventoryManager.SetClothes(currentWeaponSlot.itemInfo.item, index != 0);

        weaponController.SetWeapon(currentWeaponSlot);

        RefreshUI(index);
    }

    public bool canReload(ammoItem ammoItem, bool mainWeapon, out string message)
    {
        message = "Не подходит.";
        if (weaponSlots[1].SlotIsNull() && weaponSlots[2].SlotIsNull())
        {
            message = "У меня нет огнестрельного оружия";
        }
        else if (mainWeapon)
        {
            if (weaponSlots[2].SlotIsNull())
            {
                message = "У меня нет основного оружия.";
            }
            else
            {
                if (weaponController.FullAmmo(weaponSlots[2]))
                {
                    message = "Уже полностью заряжено."; return false;
                }
                return ((weaponSlots[2].itemInfo.item as weaponItem).ammo == ammoItem);
            }
        }
        else if (!mainWeapon)
        {
            if (weaponSlots[1].SlotIsNull())
            {
                message = "У меня нет доп оружия.";
            }
            else
            {
                if (weaponController.FullAmmo(weaponSlots[1]))
                {
                    message = "Уже полностью заряжено."; return false;
                }
                return ((weaponSlots[1].itemInfo.item as weaponItem).ammo == ammoItem);
            }
        }
        return false;
    }

    /// <summary>
    /// index - current weapon slot index
    /// </summary>
    /// <param name="index"></param>
    private void RefreshUI(int index)
    {
        if (!weaponUI[0].activeSelf && index != 0)
        {
            foreach (var item in weaponUI)
            {
                item.SetActive(true);
            }
        }
        else if (weaponUI[0].activeSelf && index == 0)
        {
            foreach (var item in weaponUI)
            {
                item.SetActive(false);
            }
        }
    }
    private void RefreshUI()
    {
        if (!currentWeaponSlot.SlotIsNull())
        {
            if (!weaponUI[0].activeSelf && currentWeaponSlot.slotType != ItemType.melee)
            {
                foreach (var item in weaponUI)
                {
                    item.SetActive(true);
                }
            }
            else if (weaponUI[0].activeSelf && currentWeaponSlot.slotType == ItemType.melee)
            {
                foreach (var item in weaponUI)
                {
                    item.SetActive(false);
                }
            }
        }
        else
        {
            if (weaponUI[0].activeSelf)
            {
                foreach (var item in weaponUI)
                {
                    item.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Switch to pistol when pislot and rifle == null
    /// Switch to rifle when pistol != null or pistol == null
    /// </summary>
    /// 
    public bool playerHasWeapon;
    public void SwitchToNew()
    {
        playerHasWeapon = true;
        if (!weaponSlots[2].SlotIsNull())
        {
            SwitchWeapon(weaponSlots[2]);
            InventoryManager.SetClothes(weaponSlots[2].itemInfo.item, true);
        }
        else if (!weaponSlots[1].SlotIsNull())
        {
            SwitchWeapon(weaponSlots[1]);
            InventoryManager.SetClothes(weaponSlots[1].itemInfo.item, true);
        }
        else
        {
            SwitchWeapon();
            if (!weaponSlots[0].SlotIsNull())
            {
                InventoryManager.SetClothes(weaponSlots[0].itemInfo.item);
            }
            playerHasWeapon = false;
        }
    }
    public bool PlayerHasAxeInHand()
    {
        if (!currentWeaponSlot.IsEmpty)
        {
            return currentWeaponSlot.itemInfo.item.name.Contains("axe");
        }
        return false;
    }
    public void ReloadFor(InventorySlot ammoSlot, int weaponSlotIndex)
    {
        var reloadedWeaponSlot = weaponSlots[weaponSlotIndex];

        if (!weaponController.itsEnable || weaponController.FullAmmo(reloadedWeaponSlot) || reloading || reloadedWeaponSlot.IsSlotBlocked) { return; }
        ReloadImageAnimator.gameObject.SetActive(true);
        ReloadImageAnimator.speed = 1 / (reloadedWeaponSlot.itemInfo.item as weaponItem).reload_time;
        reloadedWeaponSlot.IsSlotBlocked = true;
        reloading = true;

        StartCoroutine(IEEndReload(reloadedWeaponSlot, (reloadedWeaponSlot.itemInfo.item as weaponItem).reload_time));

        weaponController.ReloadWeaponFor(ammoSlot, reloadedWeaponSlot);

        weaponSlots[weaponSlotIndex] = reloadedWeaponSlot;
    }

    public void RefreshWeaponUI()
    {
        foreach (var slot in weaponSlots)
        {
            if (!slot.IsEmpty)
            {
                slot.RefreshAmmo();
            }
        }
    }
    public void ShootButtonDown()
    {
        weaponController.shootButtonDown = true;
        weaponController.PlayerShoot();
    }
    public void ShootButtonUp() => weaponController.shootButtonDown = false;
    public void ReloadButtonPress()
    {
        if (weaponController.FullAmmo() || !weaponController.PlayerHaveAmmo() || reloading || currentWeaponSlot.IsSlotBlocked) { return; }
        ReloadImageAnimator.gameObject.SetActive(true);
        weaponController.ReloadWeapon();
        ReloadImageAnimator.speed = 1 / (currentWeaponSlot.itemInfo.item as weaponItem).reload_time;
        currentWeaponSlot.IsSlotBlocked = true;
        reloading = true;
        Invoke("EndReload", (currentWeaponSlot.itemInfo.item as weaponItem).reload_time);
    }
    private void EndReload()
    {
        currentWeaponSlot.IsSlotBlocked = false;
        reloading = false;
        ReloadImageAnimator.gameObject.SetActive(false);
    }
    private IEnumerator IEEndReload(InventorySlot weaponSlot, float time)
    {
        yield return new WaitForSeconds(time);
        weaponSlot.IsSlotBlocked = false;
        reloading = false;
        ReloadImageAnimator.gameObject.SetActive(false);
    }
}
