using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GAME._scripts.Fic;
using UnityEngine;

public class WeaponShooter: MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float force;
    private void Awake()
    {
        EventBus.Subscribe<Signal_WeaponShoot>(WeaponShoot_Handle);
        EventBus.Subscribe<Signal_ChangeWeapon>(ChangeWeapon_Handler);
    }

    private void WeaponShoot_Handle(Signal_WeaponShoot shoot)
    {

        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);

        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(shoot.ShootDir * force, ForceMode2D.Force);
        }

        Destroy(newBullet, 2f);
    }

    private void ChangeWeapon_Handler(Signal_ChangeWeapon weapon)
    {
        if(weapon.weaponItem == null)
        {
            //IS HAND LOGIC

            return;
        }

        bullet = weapon.weaponItem.ammo.prefabAmmo;
        force = weapon.weaponItem.force;
    }
}