using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public struct InitPlayerMessage : NetworkMessage { }

public class ClientManager : MonoBehaviour
{
    public GameObject itemObjectPrefab;
    public ItemObjectSyncer itemObjectSyncer;

    public struct InitItemObjects : NetworkMessage
    {
        public List<ItemObjectData> items;
    }
    private void Awake()
    {
        NetworkClient.RegisterHandler<InitItemObjects>(OnInitItemObjects);
        NetworkClient.RegisterHandler<InitPlayerMessage>(OnInitPlayerMessage);
        NetworkClient.RegisterHandler<DestroyItemObjectMessage>(OnDestroyItemObjectMessage);
        NetworkClient.RegisterHandler<CreateItemObjectMessage>(OnCreateItemObject);
        NetworkClient.RegisterHandler<SetActiveObjectMessage>(OnSetActivePlayerMessage);

        SetupCanvas.SetText("–егистрируем сообщений");
    }

    private void OnSetActivePlayerMessage(SetActiveObjectMessage message)
    {
        NetworkServer.spawned.TryGetValue(message.object_netID, out var identity);
        if (identity != null)
        {
            identity.gameObject.SetActive(message.state);
        }
    }

    private void OnCreateItemObject(CreateItemObjectMessage message)
    {
        ItemObject itemObject = Instantiate(itemObjectPrefab, message.position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(message.itemInfo);
        IdentityObject identityObject = itemObject.GetComponent<IdentityObject>();
        identityObject.ID = message.ID;

    }
    private void OnDestroyItemObjectMessage(DestroyItemObjectMessage message)
    {
        IdentityManager.TryGetAtID(message.ID, out GameObject itemObject);
        if (itemObject != null)
        {
            Destroy(itemObject);
        }
    }

    public void OnInitItemObjects(InitItemObjects message)
    {
        print("ItemObjectSyncer is null? - " + ItemObjectSyncer.instance == null);
        SetupCanvas.SetText("—инхронизируем объекты");
        itemObjectSyncer.TInitItemObjects(message.items);

        print("[CLEINT] отправл€ю сообщение и том, что € (клиент) закончил синхронизацию");
        NetworkClient.Send(new ClaimSyncMessage { });
    }
    public void OnInitPlayerMessage(InitPlayerMessage message)
    {
        SetupCanvas.SetText("»нициализируем игрока");

        Initializer.Init(NetworkClient.localPlayer.transform);
        if (NetworkClient.localPlayer.isLocalPlayer)
        {
            NetworkClient.localPlayer.name = "Local Player";
        }

        SetupCanvas.Destroy();
    }
}
