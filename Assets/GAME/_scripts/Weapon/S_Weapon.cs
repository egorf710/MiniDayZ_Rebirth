using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GAME._scripts.Fic;
using JetBrains.Annotations;
using UnityEngine;

public class S_Weapon : MonoBehaviour
{
    [SerializeField] private InventorySlot currentWeaponSlot;
    [SerializeField] private weaponItem currentWeapon;

    [SerializeField] private int currentAmmo;

    public void Init()
    {

    }

    public void PressShootBtt()
    {
        
        if (currentWeapon == null)
        {
            return;
        }
        else if (currentWeaponSlot.itemInfo.durability <= 0)
        {
            DebuMessager.Mess("Оружие неисправно!", Color.red);
            return;
        }
        else if (currentAmmo <= 0)
        {
            DebuMessager.Mess("Патроны закончились!", Color.red);
            return;
        }
        currentAmmo -= 1;
        EventBus.Invoke<Signal_WeaponShoot>(new Signal_WeaponShoot(new Vector2(1, 0)));

        //Item - Damage
        currentWeaponSlot.itemInfo.durability -= currentWeaponSlot.itemInfo.item.durabilityDamage;
        //UpdateVisual
        currentWeaponSlot.itemInfo.ammo = currentAmmo;
        currentWeaponSlot.Refresh();
    }

    public void ChangeWeapon(InventorySlot weaponSlot)
    {
        if (weaponSlot.itemInfo.item == null)
        {
            //IS HAND LOGIC
            currentWeapon = null;
            currentWeaponSlot = null;

            return;
        }
        currentWeapon = weaponSlot.itemInfo.item as weaponItem;
        currentWeaponSlot = weaponSlot;
    }

    public void PressReloadBtt()
    {
        int ammoToReload = currentWeapon.mag_size - currentAmmo;

        var ammoSlot = ServiceLocator.Get<S_Inventory>().GetSlotByItem(currentWeapon.ammo);

        if(ammoSlot == null) { return; }

        int weHaveAmmo = ammoSlot.itemInfo.amount;

        if (ammoToReload <= 0) { return; }

        int ammoUsed = Mathf.Min(ammoToReload, weHaveAmmo);

        currentAmmo += ammoUsed;

        ammoSlot.itemInfo.amount -= ammoUsed;
        currentWeaponSlot.itemInfo.ammo = currentAmmo;

        ammoSlot.Refresh();
        currentWeaponSlot.RefreshAmmo();
    }
}
