using System;
using System.Collections;
using System.Collections.Generic;
using Assets._scripts.Interfaces;
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
    public struct StatetableMessage : NetworkMessage
    {
        public List<StatetableObject> statetableObjects;
    }
    private void Awake()
    {
        NetworkClient.RegisterHandler<InitItemObjects>(OnInitItemObjects);
        NetworkClient.RegisterHandler<InitPlayerMessage>(OnInitPlayerMessage);
        NetworkClient.RegisterHandler<DestroyItemObjectMessage>(OnDestroyItemObjectMessage);
        NetworkClient.RegisterHandler<CreateItemObjectMessage>(OnCreateItemObject);
        NetworkClient.RegisterHandler<SetActiveObjectMessage>(OnSetActivePlayerMessage);
        NetworkClient.RegisterHandler<SetStateMessage>(OnSetStateMessage);
        NetworkClient.RegisterHandler<InteractMessage>(OnInteractMessage);
        NetworkClient.RegisterHandler<StatetableMessage>(OnStatetableMessage);

        SetupCanvas.SetText("Регистрируем сообщений");
    }

    private void OnStatetableMessage(StatetableMessage message)
    {
        SetupCanvas.SetText("Синхронизируем состояния объектов");
        foreach (var item in message.statetableObjects)
        {
            NetworkClient.spawned.TryGetValue(item.netId, out var identity);
            if (identity != null)
            {
                identity.gameObject.TryGetComponent(out Statetable component);
                component.SetStatetable(item.state);
            }
            else
            {
                print("[CLIENT] try change state " + item.netId);
            }
        }
        print("[CLEINT] отправляю сообщение и том, что я (клиент) закончил синхронизацию");
        NetworkClient.Send(new ClaimSyncMessage { });
        SetupCanvas.Destroy();
    }

    private void OnInteractMessage(InteractMessage message)
    {
        NetworkClient.spawned.TryGetValue(message.object_netID, out var identity);
        if (identity != null)
        {
            identity.gameObject.TryGetComponent(out Interactable component);
            component.Interact();
        }
        else
        {
            print("[CLIENT] try change active state " + message.object_netID);
        }
    }

    private void OnSetStateMessage(SetStateMessage message)
    {
        NetworkClient.spawned.TryGetValue(message.object_netID, out var identity);
        if (identity != null)
        {
            identity.gameObject.GetComponent<Statetable>().SetStatetable(message.state);
            print($"[CLIENT] устанавливаю стостояние {message.state} для {message.object_netID}");
        }
        else
        {
            print("[CLIENT] try change active state " + message.object_netID);
        }
    }

    private void OnSetActivePlayerMessage(SetActiveObjectMessage message)
    {
        NetworkClient.spawned.TryGetValue(message.object_netID, out var identity);
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
        SetupCanvas.SetText("Синхронизируем предметы");
        itemObjectSyncer.TInitItemObjects(message.items);
    }
    public void OnInitPlayerMessage(InitPlayerMessage message)
    {
        SetupCanvas.SetText("Инициализируем игрока");
        Initializer.Init(NetworkClient.localPlayer.transform);
        NetworkClient.localPlayer.name = "Local Player";
    }
}
