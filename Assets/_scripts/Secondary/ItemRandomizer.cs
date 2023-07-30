using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ItemLoot
{
    public Item item;
    /// <summary>
    /// x-min   y-max
    /// </summary>
    public Vector2 amountRange;
    public int amount;
    public int maxDropCount;
    public float chance;

    public ItemLoot(ItemLoot itemLoot)
    {
        item = itemLoot.item;
        amountRange = itemLoot.amountRange;
        amount = itemLoot.amount;
        maxDropCount = itemLoot.maxDropCount;
        chance = itemLoot.chance;
    }
}

public static class ItemRandomizer
{
    public static ItemLoot GetItem(ItemLoot[] itemLoots, bool randomAmount = false)
    {
        // Общая сумма шансов выпадения
        double totalChance = 0;
        foreach (var obj in itemLoots)
        {
            totalChance += obj.chance;
        }

        // Генерируем случайное число от 0 до 1
        double randomNumber = Random.Range(0, 1f);

        // Выбираем объект на основе случайного числа и шансов выпадения
        double cumulativeChance = 0;
        foreach (var obj in itemLoots)
        {
            cumulativeChance += obj.chance / totalChance;
            if (randomNumber <= cumulativeChance)
            {
                ItemLoot itemLoot = new ItemLoot(obj);
                if (randomAmount)
                {
                    itemLoot.amount = (int)Mathf.Clamp(UnityEngine.Random.Range(obj.amountRange.x, obj.amountRange.y), 1, obj.item.item_max_amount);
                }
                return itemLoot;
                //break;
            }
        }
        
        return new ItemLoot();
    }

    public static ItemLoot[] GetItems(ItemLoot[] itemLoots, int count = 2, bool randomAmount = false)
    {
        // Количество объектов, которые нужно выбрать
        int countToSelect = count;
        List<ItemLoot> items = new List<ItemLoot>(count);
        Dictionary<ItemLoot, int> dropedItems = new Dictionary<ItemLoot, int>();
        //if(count > itemLoots.Length) 
        //{
        //    Debug.LogError($"GET ITEMS, {itemLoots.Length} < {count}, itemLoots.Length < countToGet");
        //    return new ItemLoot[0]; 
        //}
        foreach (var item in itemLoots)
        {
            dropedItems.Add(item, 0);
        }
        // Выбираем объекты на основе их шансов выпадения
        for (int i = 0; i < countToSelect; i++)
        {
            // Общая сумма шансов выпадения
            double totalChance = 0;
            foreach (var obj in itemLoots)
            {
                totalChance += obj.chance;
            }

            // Генерируем случайное число от 0 до 1
            double randomNumber = Random.Range(0, 1f);

            // Выбираем объект на основе случайного числа и шансов выпадения
            double cumulativeChance = 0;
            foreach (var obj in itemLoots)
            {
                cumulativeChance += obj.chance / totalChance;
                if (randomNumber <= cumulativeChance && dropedItems[obj] < obj.maxDropCount)
                {
                    ItemLoot itemLoot = new ItemLoot(obj);
                    if (randomAmount)
                    {
                        itemLoot.amount = (int)Mathf.Clamp(UnityEngine.Random.Range(obj.amountRange.x, obj.amountRange.y), 1, obj.item.item_max_amount);
                    }
                    items.Add(itemLoot);
                    dropedItems[obj] = dropedItems[obj] += 1;
                    break;
                }
            }
        }

        return items.ToArray();
    }
}