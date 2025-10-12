using Assets._scripts.World;
using Microsoft.VisualBasic.Devices;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemObject;

public class PlayerNetwork : NetworkBehaviour, Initable, AliveTarget
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] public ServerManager serverManager;
    [SerializeField] WeaponController MyWeaponController;
    [Space]
    [Header("SyncData")]
    public PlayerAnimator playerAnimator;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            TargetManager.AddTarget(this);
            gameObject.layer = 8;
        }
        serverManager = FindObjectOfType<ServerManager>();
    }
    public void Init(WeaponController myWeaponController)
    {
        networkManager = FindObjectOfType<NetworkManager>();

        MyWeaponController = myWeaponController;


    }
    //ClientPRC
    public void ServerPlayerShoot()
    {
        MyWeaponController.Shoot();
    }

    public Transform getTransform()
    {
        return transform;
    }

    public void TakeDamage(int damage, int code = 0)
    {
        GetComponent<PlayerCharacteristics>().TakeDamage(damage, code);
    }

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
            SetMyClothes(playerAnimator.GetAnimClothesData());
        }
    }


    [ClientRpc]
    public void SetMyClothes(string cloches)
    {
        playerAnimator.SetAnimClohesData(cloches);
    }
    [Command]
    public void CMDAnimSync(bool interact, bool shoot, float speed, Vector2 animDir) //TO DO if server => playerAnimator.interact, playerAnimator.shoot, playerAnimator.speed, playerAnimator.animationDir
    {

        CRAnimSyncs(interact, shoot, speed, animDir);
    }
    [ClientRpc]
    public void CRAnimSyncs(bool interact, bool shoot, float speed, Vector2 animDir)
    {
        if (!isLocalPlayer)
        {
            //print(name + " ok i synced :/ ");
            playerAnimator.INTERACT = interact;
            playerAnimator.shoot = shoot;
            playerAnimator.speed = speed;
            playerAnimator.animationDir = animDir;
        }
    }
    [Header("Inventory")]
    [SerializeField] private GameObject itemObjectPrefab;
    public void TOCMDSpawnItemObject(Vector3 position, ItemObject.NetworkItemInfo itemInfo)
    {
        CMDSpawnItemObject(position, itemInfo);
    }
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
        CLTSynceItemObjects(itemObjectDatas);
    }
    [ClientRpc]
    public void CLTSynceItemObjects(List<ItemObjectData> itemObjectDatas)
    {
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
        bulletPrefab = weaponItem.ammo.prefabAmmo;
        force = weaponItem.force;
        CLTSetWeapon(weaponItemName);
    }
    [ClientRpc]
    private void CLTSetWeapon(string weaponItemName)
    {
        if (isServer) { return; }
        weaponItem = InventoryManager.GetItemByName(weaponItemName) as weaponItem;
        bulletPrefab = weaponItem.ammo.prefabAmmo;
        force = weaponItem.force;
    }
    [Command]
    public void CMDShoot(Vector2 targetPos, int damage)
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Init(targetPos, damage, true, netId); 
        CLTShoot(targetPos, damage);
    }
    [ClientRpc]
    public void CLTShoot(Vector2 targetPos, int damage)
    {
        if (isServer) { return; }
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Init(targetPos, damage, false, netId);
    }

    [Command]
    public void TakeDamageTo(int damage, uint netId, int code)
    {
        //serverManager.GetPlayer(netId).TRTakeDame(damage, code);
    }
    [Command(requiresAuthority = false)]
    public void CMDTakeDamage(int damage, int code)
    {
        CLTTakeDame(damage, code);
    }
    [ClientRpc]
    private void CLTTakeDame(int damage, int code)
    {
        if (!isLocalPlayer) { return; }
        TakeDamage(damage, code);
    }

    public void SetActivePlayerSkin(bool b)
    {
        CMDSetActivePlayerSkin(b);
    }
    [Command]
    private void CMDSetActivePlayerSkin(bool b)
    {
        CLTSetActivePlayerSkin(b);
    }
    [ClientRpc]
    public void CLTSetActivePlayerSkin(bool b)
    {
        transform.GetChild(0).gameObject.GetComponent<PlayerAnimator>().SetActive(b);
        GetComponent<PlayerMove>().enabled = b;
        GetComponent<WeaponController>().enabled = b;
        if(!b)
        {
            TargetManager.RemoveTarget(this);
        }
        else
        {
            TargetManager.AddTarget(this);
        }
    }

    public void TOCMDSetBleedingParticle(int power)
    {
        CMDSetBleedingParticle(power);
    }
    [Command]
    private void CMDSetBleedingParticle(int power)
    {
        CLTSetBleedingParticle(power);
    }
    [ClientRpc]
    private void CLTSetBleedingParticle(int power)
    {
        if (isServer) { return; }
        GetComponent<PlayerCharacteristics>().SetBleedingParticle(power);
    }
}
public class PlayerData
{
    public string name;
    public string clothes_data;
}
