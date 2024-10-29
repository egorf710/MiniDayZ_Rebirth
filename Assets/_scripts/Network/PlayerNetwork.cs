using Assets._scripts.World;
using Microsoft.VisualBasic.Devices;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            print("server update net status");
            RefreshMyStatus();
        }
    }
    [ClientRpc]
    public void RefreshMyStatus()
    {
        if (isServer)
        {
            print("server player send data");
            SetMyClothes(playerAnimator.GetAnimClothesData());
        }
    }


    [ClientRpc]
    public void SetMyClothes(string cloches)
    {
        print(name + " is set clothes");
        playerAnimator.SetAnimClohesData(cloches);
        
    }
    [Command]
    public void CMDAnimSync(bool interact, bool shoot, float speed, Vector2 animDir) //TO DO if server => playerAnimator.interact, playerAnimator.shoot, playerAnimator.speed, playerAnimator.animationDir
    {
        print(name + " sync my anim pls :(");
        CRAnimSyncs(interact, shoot, speed, animDir);
    }
    [ClientRpc]
    public void CRAnimSyncs(bool interact, bool shoot, float speed, Vector2 animDir)
    {
        if (!isLocalPlayer)
        {
            print(name + " ok i synced :/ ");
            playerAnimator.interact = interact;
            playerAnimator.shoot = shoot;
            playerAnimator.speed = speed;
            playerAnimator.animationDir = animDir;
        }
    }
}
public class PlayerData
{
    public string name;
    public string clothes_data;
}
