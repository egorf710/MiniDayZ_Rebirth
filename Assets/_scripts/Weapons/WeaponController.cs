using Assets._scripts.Interfaces;
using Assets._scripts.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineInternal;

public class WeaponController : MonoBehaviour
{
    [SerializeField] public ZedAimOutline aimOutline;
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
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private float force;
    [SerializeField] private PlayerNetwork myPlayerNetwork;
    [Space]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletPrefab2;

    private Vector3 TARGET_OFFSET = new Vector3(0, 0.5f);
    private int bullet_damage = 0;
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

        bulletPrefab = (currentWeaponSlot.itemInfo.item as weaponItem).ammo.prefabAmmo;
        force = (currentWeaponSlot.itemInfo.item as weaponItem).force;
        bullet_damage = weaponItem.damage;

        if (myPlayerNetwork.isLocalPlayer)
        {
            myPlayerNetwork.CMDSetWeapon("Items/" + weaponItem.item_path + weaponItem.name);
        }
        StartCoroutine(WeaponUpdate());

    }

    private IEnumerator WeaponUpdate()
    {
        while(currentWeaponSlot != null && !currentWeaponSlot.SlotIsNull())
        {
            AliveTarget[] zedBases = TargetManager.GetTargetsAtPoint(transform.position, noticedRadius);
            if (zedBases.Length > 0) {

                Transform target = zedBases.OrderByDescending(v2 => GetDistance(transform.position, v2.getTransform().position)).Last().getTransform();


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
                PlayerShoot();
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
            int feeledAmmo = 0;
            bool clearSlot = false;
            if (ammo_in_slot > need_ammo)
            {
                int colled = ammo_in_slot - need_ammo; // 18-4 = 14
                ammo_slot.itemInfo.amount = colled; // 14
                //included_ammo = weaponItem.mag_size; //full

                feeledAmmo = weaponItem.mag_size;

                
            }
            else
            {
                //included_ammo = ammo_in_slot;
                feeledAmmo = ammo_in_slot;
                clearSlot = true;
                ammo_slot.ClearSlot();
            }
            ammo_slot.Refresh();
            currentWeaponSlot.itemInfo.ammo = included_ammo;
            currentWeaponSlot.RefreshAmmo();
            myPlayerNetwork.ReloadMe(feeledAmmo, weaponItem.reload_time, clearSlot);
        }
    }
    public IEnumerator FeelAmmo(int ammo, float realodTime, bool clearSlot)
    {
        yield return new WaitForSeconds(realodTime);
        included_ammo = ammo;
        if(clearSlot) { InventoryManager.GetSlotByItem(weaponItem.ammo).ClearSlot(); }
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
    public void PlayerShoot()
    {
        myPlayerNetwork.ServerPlayerShoot();
    }
    public void Shoot()
    {
        if (Time.time > nextTime && included_ammo > 0)
        {
            nextTime = Time.time + fireRate;
            included_ammo--;
            currentWeaponSlot.itemInfo.ammo = included_ammo;
            currentWeaponSlot.RefreshAmmo();
            aim_range += weaponItem.spread.x;

            {

                Vector3 dir = playerAnimator.animationDir;
                Vector3 targetPos = Vector3.zero;
                float distanceToTarget = force;
                if (target != null)
                {

                    targetPos = target.position + TARGET_OFFSET;
                    dir = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);

                    distanceToTarget = GetDistance(transform.position, targetPos); //TODO неправильно задаётся точка остановки пули когда стреляю во врага
                }

                Vector2 goal = dir.normalized * distanceToTarget + transform.position;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, force, 3);

                if(hit.collider != null && GetDistance(transform.position, hit.point) < GetDistance(transform.position, targetPos))
                {
                    //hit
                    Bullet bullet = Instantiate(bulletPrefab2, transform.position, Quaternion.identity).GetComponent<Bullet>();
                    bullet.Init(hit.point, distanceToTarget);
                    myPlayerNetwork.CMDShoot(hit.point);
                }
                else
                {
                    //miss
                    Bullet bullet = Instantiate(bulletPrefab2, transform.position, Quaternion.identity).GetComponent<Bullet>();
                    if (target == null)
                    {
                        bullet.Init(goal, distanceToTarget);
                        myPlayerNetwork.CMDShoot(goal);
                    }
                    else
                    {
                        bullet.Init(targetPos, target.GetComponent<VulnerableObject>(), bullet_damage, distanceToTarget);
                        myPlayerNetwork.CMDShoot(targetPos);
                    }
                }

/*                Vector2 dir;
                //float TargetAngle = 0;
                if (target != null)
                {
                    //TargetAngle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg;
                    dir = new Vector2(target.position.y - transform.position.y, target.position.x - transform.position.x);
                }
                else
                {
                    //TargetAngle = Mathf.Atan2(playerMove.moveDirection.y, playerMove.moveDirection.x) * Mathf.Rad2Deg;
                    dir = playerMove.moveDirection;
                }*/

                //TODO

                //TO DO


/*                if (target != null) 
                {
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position);

                    if (hit.collider == null)/// TO DO!!!!!!!!!!!!!!!!!
                    {
                        Bullet bullet = Instantiate(bulletPrefab2, transform.position + TARGET_OFFSET, Quaternion.identity).GetComponent<Bullet>();
                        bullet.Init(target.position + TARGET_OFFSET);
                    }
                    else
                    {
                        Bullet bullet = Instantiate(bulletPrefab2, transform.position + TARGET_OFFSET, Quaternion.identity).GetComponent<Bullet>();
                        bullet.Init(hit.point);
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position + TARGET_OFFSET, dir, force);

                    if (hit.collider != null)
                    {
                        Bullet bullet = Instantiate(bulletPrefab2, transform.position + TARGET_OFFSET, Quaternion.identity).GetComponent<Bullet>();
                        bullet.Init(hit.point);
                    }
                    else
                    {
                        Bullet bullet = Instantiate(bulletPrefab2, transform.position + TARGET_OFFSET, Quaternion.identity).GetComponent<Bullet>();
                        bullet.Init(dir.normalized * force);
                    }
                }*/
            }
        }
    }
    private float GetDistance(Vector2 v1, Vector3 v2)
    {
        return Vector2.Distance(v1, v2);
    }

    //Для проверки готовности игрока к выстрелу на сервере
    public bool PlayerReadyToShoot()
    {
        return (Time.time > nextTime && included_ammo > 0);
    }
    public bool PlayerReadyToRealod()
    {
        return (InventoryManager.GetSlotByItem(weaponItem.ammo) != null) && InventoryManager.GetSlotByItem(weaponItem.ammo).itemInfo.amount > 0;
    }
}
