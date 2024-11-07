using Assets._scripts.World;
using Microsoft.VisualBasic.Devices;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static ItemObject;

public class PlayerNetwork : NetworkBehaviour, Initable, AliveTarget
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] WeaponController MyWeaponController;
    [Space]
    [Header("SyncData")]
    public PlayerAnimator playerAnimator;

    public void Init(WeaponController myWeaponController)
    {
        networkManager = FindObjectOfType<NetworkManager>();
        MyWeaponController = myWeaponController;
    }
    //ClientPRC
    public void ServerPlayerShoot()
    {
        if (MyWeaponController.PlayerReadyToShoot())
        {
            MyWeaponController.Shoot();
        }
    }

    //Command
    public void ReloadMe(int ammo, float realoadTime, bool clearSlot = false)
    {
        if (MyWeaponController.PlayerReadyToRealod())
        {
            StartCoroutine(MyWeaponController.FeelAmmo(ammo, realoadTime, clearSlot));
        }
    }

    public Transform getTransform()
    {
        return transform;
    }

    public void TakeDamage(int damage) { }

    public uint getNetID()
    {
        return netId;
    }

    public void Init(Transform player)
    {
        CMDAnimSync(playerAnimator.interact, playerAnimator.shoot, playerAnimator.speed, playerAnimator.animationDir);
        networkManager = FindObjectOfType<NetworkManager>();
        MyWeaponController = player.GetComponent<WeaponController>();
    }


    public void NetUpdate()
    {
        if(isServer)
        {
            //print("server update net status");
            RefreshMyStatus();
        }
    }
    [Command]
    public void CMDUpdateMyClohes(string clotheData)
    {
        SetMyClothes(clotheData);
    }
    [ClientRpc]
    public void RefreshMyStatus()
    {
        if (isServer)
        {
            //print("server player send data");
            SetMyClothes(playerAnimator.GetAnimClothesData());
        }
    }


    [ClientRpc]
    public void SetMyClothes(string cloches)
    {
        //print(name + " is set clothes");
        playerAnimator.SetAnimClohesData(cloches);
    }
    [Command]
    public void CMDAnimSync(bool interact, bool shoot, float speed, Vector2 animDir) //TO DO if server => playerAnimator.interact, playerAnimator.shoot, playerAnimator.speed, playerAnimator.animationDir
    {
        //print(name + " sync my anim pls :(");
        CRAnimSyncs(interact, shoot, speed, animDir);
    }
    [ClientRpc]
    public void CRAnimSyncs(bool interact, bool shoot, float speed, Vector2 animDir)
    {
        if (!isLocalPlayer)
        {
            //print(name + " ok i synced :/ ");
            playerAnimator.interact = interact;
            playerAnimator.shoot = shoot;
            playerAnimator.speed = speed;
            playerAnimator.animationDir = animDir;
        }
    }
    [Header("Inventory")]
    [SerializeField] private GameObject itemObjectPrefab;
    [Command]
    public void CMDSpawnItemObject(Vector3 position, ItemObject.NetworkItemInfo itemInfo)
    {
        ItemObject itemObject = Instantiate(itemObjectPrefab, position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(itemInfo);
        IdentityObject identityObject = itemObject.GetComponent<IdentityObject>();
        IdentityManager.SetObjectID(ref identityObject);

        CLTSpawnItemObject(position, itemInfo, identityObject.ID);
    }
    [ClientRpc]
    public void CLTSpawnItemObject(Vector3 position, ItemObject.NetworkItemInfo itemInfo, int ID)
    {
        if (isServer) { return; }
        ItemObject itemObject = Instantiate(itemObjectPrefab, position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(itemInfo);
        IdentityObject identityObject = itemObject.GetComponent<IdentityObject>();
        identityObject.ID = ID;

    }

    [Command]
    public void CMDSynceItemObjects(List<ItemObjectData> itemObjectDatas)
    {
        //print("send IO datas");
        CLTSynceItemObjects(itemObjectDatas);
    }
    [ClientRpc]
    public void CLTSynceItemObjects(List<ItemObjectData> itemObjectDatas)
    {
        //print("recive IO datas");
        ItemObjectSyncer.InitItemObjects(itemObjectDatas);
    }


    [Command]
    public void CMDDestroyItemObjectAtID(int ID)
    {
        CLTDestroyItemObjectAtID(ID);
    }
    [ClientRpc]
    public void CLTDestroyItemObjectAtID(int ID)
    {
        IdentityManager.TryGetAtID(ID, out GameObject go);
        if (go != null)
        { 
            Destroy(go);
        }
    }

    [SerializeField] private weaponItem weaponItem;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float force;

    [Command]
    public void CMDSetWeapon(string weaponItemName)
    {
        weaponItem = InventoryManager.GetItemByName(weaponItemName) as weaponItem;
        //bulletPrefab = weaponItem.ammo.prefabAmmo;
        force = weaponItem.force;
    }
    [Command]
    public void CMDShoot(Vector2 targetPos)
    {
        CLTShoot(targetPos);
    }
    [ClientRpc]
    public void CLTShoot(Vector2 targetPos)
    {
        if (isLocalPlayer) { return; }
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Init(targetPos, Vector2.Distance(transform.position, targetPos));
    }
}
public class PlayerData
{
    public string name;
    public string clothes_data;
}
