using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/weapon Item", fileName = "new weapon Item")]
public class weaponItem : Item
{
    public Sprite fullSprite;
    public int durability = 100;
    public int damage;
    public float shot = 1;
    public float punch_range;
    public float reload_time;
    public Vector2 spread = new Vector2(2, 14);
    public float aim_speed = 6f;
    public float fire_rate = 1f;
    public float force = 1000;
    public int mag_size;
    public float scope_range = 5;
    public float scope_baff = 1;
    public ammoItem ammo;
    public AudioClip[] shoot_clip;
    public AudioClip reload_clip;
}
