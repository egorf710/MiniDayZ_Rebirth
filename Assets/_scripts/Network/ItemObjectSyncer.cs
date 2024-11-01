using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemObject;

public class ItemObjectSyncer : MonoBehaviour, Initable
{
    public List<ItemObject> regItemObject;
    public PlayerNetwork playerNetwork;
    public static ItemObjectSyncer instance;
    public GameObject itemObjectPrefab;
    void Awake()
    {
        instance = this;
    }

    public void Init(Transform player)
    {
        playerNetwork = player.GetComponent<PlayerNetwork>();
        if (playerNetwork.isServer)
        {
            regItemObject = FindObjectsOfType<ItemObject>(true).ToList();
            for (int i = 0; i < regItemObject.Count; i++)
            {
                IdentityObject identityObject = regItemObject[i].GetComponent<IdentityObject>();
                IdentityManager.SetObjectID(ref identityObject);
            }
        }
    }

    public void NetUpdate()
    {
        if(playerNetwork == null)
        {
            playerNetwork = NetworkClient.localPlayer.GetComponent<PlayerNetwork>();
        }
        if(NetworkClient.localPlayer.isServer)
        {
            regItemObject = FindObjectsOfType<ItemObject>(true).ToList();
            List<ItemObjectData> itemObjectDatas = new List<ItemObjectData>();

            foreach(var regObj in regItemObject)
            {
                itemObjectDatas.Add(new ItemObjectData()
                {
                   NetworkItemInfo = new ItemObject.NetworkItemInfo(regObj.itemInfo),
                   pos = regObj.GetPosition(),
                   ID = regObj.GetComponent<IdentityObject>().ID,
                });
            }

            playerNetwork.CMDSynceItemObjects(itemObjectDatas);

        }
    }
    public static void InitItemObjects(List<ItemObjectData> itemObjectDatas)
    {
        instance.TInitItemObjects(itemObjectDatas);
    }
    private void TInitItemObjects(List<ItemObjectData> newItemObjectDatas)
    {
        if (playerNetwork.isServer) { return; }
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
