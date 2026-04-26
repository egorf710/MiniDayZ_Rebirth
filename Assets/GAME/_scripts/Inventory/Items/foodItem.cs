using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/food Item", fileName = "new food Item")]
public class foodItem : Item
{
    public int food_point;
    public int water_point;
    public int heal_point;
    public int temperature_point;
    public Item cooked;
}
