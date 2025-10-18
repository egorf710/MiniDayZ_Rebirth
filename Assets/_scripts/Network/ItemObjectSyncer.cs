using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;



public class ItemObjectSyncer : MonoBehaviour
{
    private List<ItemObject> regItemObject;
    public GameObject itemObjectPrefab;
    public static ItemObjectSyncer instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (NetworkServer.active)
        {
            iInit();
        }
    }
    public void iInit()
    {
        regItemObject = FindObjectsOfType<ItemObject>(true).ToList();
        for (int i = 0; i < regItemObject.Count; i++)
        {
            IdentityObject identityObject = regItemObject[i].GetComponent<IdentityObject>();
            IdentityManager.SetObjectID(ref identityObject);
        }
    }

    public static List<ItemObjectData> GetItemObjectData()
    {
        return instance.iGetItemObjectData();
    }
    public List<ItemObjectData> iGetItemObjectData()
    {
        var regItemObject = FindObjectsOfType<ItemObject>(true).ToList();
        List<ItemObjectData> itemObjectDatas = new List<ItemObjectData>();

        foreach (var regObj in regItemObject)
        {
            itemObjectDatas.Add(new ItemObjectData()
            {
                NetworkItemInfo = new ItemObject.NetworkItemInfo(regObj.itemInfo),
                pos = regObj.GetPosition(),
                ID = regObj.GetComponent<IdentityObject>().ID,
            });
        }
        return itemObjectDatas;
    }

    public static void InitItemObjects(List<ItemObjectData> newItemObjectDatas)
    {
        instance.TInitItemObjects(newItemObjectDatas);
    }
    public void TInitItemObjects(List<ItemObjectData> newItemObjectDatas)
    {

        print("init IO datas");
        DsrtMyItemObjects();
        List<ItemObjectData> itemObjectDatas = newItemObjectDatas;

        foreach (var item in itemObjectDatas)
        {
            ItemObject itemObject = Instantiate(itemObjectPrefab, item.pos, Quaternion.identity).GetComponent<ItemObject>();
            itemObject.Set(item.NetworkItemInfo);
            itemObject.GetComponent<IdentityObject>().ID = item.ID;
        }
    }
    private void DsrtMyItemObjects()
    {
        regItemObject = FindObjectsOfType<ItemObject>(true).ToList();
        foreach (var regObj in regItemObject)
        {
            Destroy(regObj.gameObject);
        }
    }

}


[Serializable]
public class ItemObjectData
{
    public ItemObject.NetworkItemInfo NetworkItemInfo;
    public Vector3 pos;
    public int ID;
}
