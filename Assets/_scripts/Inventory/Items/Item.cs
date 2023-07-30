using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { def, head, body, pants, backpack, shield, rifle, pistol, melee, food}
[CreateAssetMenu(menuName = "Items/defalt Item", fileName = "new default Item")]
public class Item : ScriptableObject
{
    public string item_name;
    public string item_path;
    [TextArea(3, 255)]
    public string item_description;
    public ItemType item_type;
    public Sprite item_sprite;
    public Sprite item_drop_sprite;

    public bool stacable;
    public int item_max_amount;
    public bool use_durability;
    public int slot_count;

    [Serializable]
    public struct Recipe
    {
        public Item item;
        public int amount;
    }
    public List<Recipe> recipes = new List<Recipe>();

}
