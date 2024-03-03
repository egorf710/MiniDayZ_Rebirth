using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct DropLootMassage
{
    public ItemLootData[] loot;
    public Vector2 pos;
}
public struct ItemLoot
{
    public Item item;
    /// <summary>
    /// x-min   y-max
    /// </summary>
    public Vector2 amountRange;
    public Vector2 durabilityRange;

    public int maxDropCount;
    public float chance;

    public ItemLoot(ItemLoot itemLoot)
    {
        item = itemLoot.item;
        amountRange = itemLoot.amountRange;
        durabilityRange = itemLoot.durabilityRange;
        maxDropCount = itemLoot.maxDropCount;
        chance = itemLoot.chance;
    }
}
public struct ItemLootData
{
    public string itemPath;
    public int amount;
    public int durability;

    public ItemLootData(ItemLootData itemLootData)
    {
        itemPath = itemLootData.itemPath;
        amount = itemLootData.amount;
        durability = itemLootData.durability;
    }
}

public static class ItemRandomizer
{
    public static ItemLootData GetItem(ItemLoot[] itemLoots, bool randomAmountAndDur = false)
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
                ItemLootData itemLoot = new ItemLootData();
                if (randomAmountAndDur)
                {
                    itemLoot.amount = (int)Mathf.Clamp(UnityEngine.Random.Range(obj.amountRange.x, obj.amountRange.y), 1, obj.item.item_max_amount);
                    itemLoot.durability = (int)Mathf.Clamp(UnityEngine.Random.Range(obj.durabilityRange.x, obj.durabilityRange.y), 1, 100);
                }
                itemLoot.itemPath = obj.item.item_path + obj.item.name;
                return itemLoot;
                //break;
            }
        }
        
        return new ItemLootData();
    }

    public static ItemLootData[] GetItems(ItemLoot[] itemLoots, int count = 2, bool randomAmountAdnDur = false)
    {
        // Количество объектов, которые нужно выбрать
        int countToSelect = count;
        List<ItemLootData> items = new List<ItemLootData>(count);
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
                    ItemLootData itemLoot = new ItemLootData();
                    if (randomAmountAdnDur)
                    {
                        itemLoot.amount = (int)Mathf.Clamp(UnityEngine.Random.Range(obj.amountRange.x, obj.amountRange.y), 1, obj.item.item_max_amount);
                        itemLoot.durability = (int)Mathf.Clamp(UnityEngine.Random.Range(obj.durabilityRange.x, obj.durabilityRange.y), 1, 100);
                    }
                    itemLoot.itemPath = obj.item.item_path + obj.item.name;
                    items.Add(itemLoot);
                    dropedItems[obj] = dropedItems[obj] += 1;
                    break;
                }
            }
        }


        return items.ToArray();
    }
}