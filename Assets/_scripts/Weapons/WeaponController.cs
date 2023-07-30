using Assets._scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private ZedAimOutline aimOutline;
    [SerializeField] private float noticedRadius;
    [SerializeField] private InventorySlot currentWeaponSlot;
    [SerializeField] private weaponItem weaponItem;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform target;
    [SerializeField] private int included_ammo;
    public bool shootButtonDown;
    [SerializeField] private float spread;
    [SerializeField] private float nextTime = 0f;
    [SerializeField] private float fireRate;
    [SerializeField] private float aim_range;
    public void SetWeapon(InventorySlot inventorySlot)
    {
        if(inventorySlot == null || (inventorySlot != null && inventorySlot.slotType == ItemType.melee))
        {
            currentWeaponSlot = null;
            weaponItem = null;
            target = null;
            aimOutline.HideAndStop();
            return;
        }
        currentWeaponSlot = inventorySlot;
        weaponItem = currentWeaponSlot.itemInfo.item as weaponItem;
        included_ammo = currentWeaponSlot.itemInfo.ammo;
        fireRate = weaponItem.fire_rate / 60;
        fireRate = 1 / fireRate;

        StartCoroutine(WeaponUpdate());
    }

    private IEnumerator WeaponUpdate()
    {
        while(currentWeaponSlot != null && !currentWeaponSlot.SlotIsNull())
        {
            ZedBase[] zedBases = ZedsManager.GetZedsAtPoint(transform.position, noticedRadius);
            if (zedBases.Length > 0) {
                Transform target = zedBases.OrderByDescending(v2 => GetDistance(transform.position, v2.transform.position)).Last().transform;

                if (this.target == null && target != null || (target != null && target != this.target))
                {
                    aimOutline.SetTarget(target);
                    this.target = target;
                }
                else if(target == null)
                {
                    aimOutline.HideAndStop();
                    this.target = null;
                }
            }
            else
            {
                aimOutline.HideAndStop();
                this.target = null;
            }
            if (shootButtonDown)
            {
                Shoot();
            }

            aim_range -= Time.deltaTime * weaponItem.aim_speed * 9;
            aim_range = Mathf.Clamp(aim_range, 0f, weaponItem.spread.y);
            aimOutline.DrawAimImageByAimRange(aim_range);

            yield return new WaitForSeconds(0.35f);
        }
        aimOutline.HideAndStop();
    }
    public void ReloadWeapon()
    {
        {
            included_ammo = currentWeaponSlot.itemInfo.ammo;

            InventorySlot ammo_slot = InventoryManager.GetSlotByItem(weaponItem.ammo);
            int ammo_in_slot = ammo_slot.itemInfo.amount; //скок есть в инвентаре 18
            int need_ammo = weaponItem.mag_size - currentWeaponSlot.itemInfo.ammo; //скоко нужно зарядить 6-2 = 4
            if (ammo_in_slot > need_ammo)
            {
                int colled = ammo_in_slot - need_ammo; // 18-4 = 14
                ammo_slot.itemInfo.amount = colled; // 14
                included_ammo = weaponItem.mag_size; //full
            }
            else
            {
                included_ammo = ammo_in_slot;
                ammo_slot.ClearSlot(); 
            }
            ammo_slot.Refresh();
            currentWeaponSlot.itemInfo.ammo = included_ammo;
            currentWeaponSlot.Refresh();
        }
    }
    public bool FullAmmo()
    {
        return (weaponItem != null && weaponItem.mag_size == included_ammo);
    }
    public bool PlayerHaveAmmo()
    {
        InventorySlot ammo_slot = InventoryManager.GetSlotByItem(weaponItem.ammo);
        return ammo_slot != null && !ammo_slot.SlotIsNull();
    }
    public void Shoot()
    {
        if (Time.time > nextTime && included_ammo > 0)
        {
            nextTime = Time.time + fireRate;
            included_ammo--;
            currentWeaponSlot.itemInfo.ammo = included_ammo;
            aim_range += weaponItem.spread.x;

            {

            }
        }
    }
    private float GetDistance(Vector2 v1, Vector3 v2)
    {
        return Vector2.Distance(v1, v2);
    }
}
