using Assets._scripts.World;
using Microsoft.VisualBasic.Devices;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour, AliveTarget
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] WeaponController MyWeaponController;

    public void Init(WeaponController myWeaponController)
    {
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
}
