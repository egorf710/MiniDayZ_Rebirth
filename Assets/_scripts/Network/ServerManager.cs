using Assets._scripts.Interfaces;
using Assets._scripts.World;
using JetBrains.Annotations;
using Microsoft.VisualBasic;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static ClientManager;

public struct ReadyToSyncMessage : NetworkMessage { }
public struct ClaimSyncMessage : NetworkMessage { }
public struct DestroyItemObjectMessage : NetworkMessage { public int ID; }
public struct CreateItemObjectMessage : NetworkMessage { public Vector3 position; public ItemObject.NetworkItemInfo itemInfo; public int ID; }
public struct TeleportPlayerToMessage : NetworkMessage { public Vector3 position; }
public struct TeleportPlayerToSpawnMessage : NetworkMessage {}
public struct SetActiveObjectMessage : NetworkMessage { public uint object_netID; public bool state; };
public struct DropLootMessage : NetworkMessage { public ItemLootData[] loot; public Vector2 pos; }
public struct SetStateMessage : NetworkMessage { public uint object_netID; public int state; }
public struct InteractMessage : NetworkMessage { public uint object_netID; }

public class ServerManager : NetworkBehaviour
{
    public GameManager gameManager;
    public static ServerManager instance;
    public List<NetPlayerCase> netPlayers = new List<NetPlayerCase>();
    public GameObject itemObjectPrefab;

    private void Awake()
    {
        instance = this;
        gameManager = FindObjectOfType<GameManager>();

        if (!NetworkServer.active) { this.enabled = false; }

        NetworkServer.RegisterHandler<ReadyToSyncMessage>(OnReadyToSync);
        NetworkServer.RegisterHandler<ClaimSyncMessage>(OnClaimSync);
        NetworkServer.RegisterHandler<DestroyItemObjectMessage>(OnDestroyItemObject);
        NetworkServer.RegisterHandler<CreateItemObjectMessage>(OnCreateItemObject);
        NetworkServer.RegisterHandler<TeleportPlayerToMessage>(OnTeleportPlayerToMessage);
        NetworkServer.RegisterHandler<TeleportPlayerToSpawnMessage>(OnTeleportPlayerToSpawnMessage);
        NetworkServer.RegisterHandler<SetActiveObjectMessage>(OnSetActivePlayerMessage);
        NetworkServer.RegisterHandler<DropLootMessage>(OnDropLootMessage);
        NetworkServer.RegisterHandler<SetStateMessage>(OnSetStateMessage);
        NetworkServer.RegisterHandler<InteractMessage>(OnInteractMessage);
    }

    private void OnInteractMessage(NetworkConnectionToClient client, InteractMessage message)
    {
        NetworkServer.spawned.TryGetValue(message.object_netID, out var identity);
        if (identity != null)
        {
            identity.gameObject.TryGetComponent(out Interactable component);
            if (component != null)
            {
                if (component.IsInteractable(out string msg))
                {
                    component.Interact();
                    NetworkServer.SendToAll(new InteractMessage { object_netID = message.object_netID });
                }

                print("[SERVER] " + msg);
            }


        }
        else
        {
            print("try change active state " + message.object_netID);
        }
    }

    private void OnSetStateMessage(NetworkConnectionToClient client, SetStateMessage message)
    {
        NetworkServer.spawned.TryGetValue(message.object_netID, out var identity);
        if (identity != null)
        {
            identity.gameObject.GetComponent<Statetable>().SetStatetable(message.state);

            NetworkServer.SendToAll(new SetStateMessage { object_netID = message.object_netID, state = message.state });
        }
        else
        {
            print("try change active state " + message.object_netID);
        }
    }

    private void OnDropLootMessage(NetworkConnectionToClient client, DropLootMessage message)
    {
        Vector2 pos = message.pos;
        ItemLootData[] itemLoots = message.loot;

        InventoryManager.SpawnLoot(itemLoots, pos);
    }

    private void OnSetActivePlayerMessage(NetworkConnectionToClient client, SetActiveObjectMessage message)
    {
        NetworkServer.spawned.TryGetValue(message.object_netID, out var identity);
        if(identity != null)
        {
            identity.gameObject.SetActive(message.state);

            NetworkServer.SendToAll(new SetActiveObjectMessage { object_netID = message.object_netID, state = message.state });
        }
        else
        {
            print("try change active state " + message.object_netID);
        }
    }

    private void OnTeleportPlayerToSpawnMessage(NetworkConnectionToClient client, TeleportPlayerToSpawnMessage message)
    {
        Transform startPos = GameObject.Find("SPAWNPOINT").transform;
        GetPlayer(client).transform.position = startPos.position;
    }

    private void OnTeleportPlayerToMessage(NetworkConnectionToClient client, TeleportPlayerToMessage message)
    {
        GetPlayer(client).transform.position = message.position;
    }

    private void OnCreateItemObject(NetworkConnectionToClient client, CreateItemObjectMessage message)
    {
        ItemObject itemObject = Instantiate(itemObjectPrefab, message.position, Quaternion.identity).GetComponent<ItemObject>();
        itemObject.Set(message.itemInfo);
        IdentityObject identityObject = itemObject.GetComponent<IdentityObject>();
        IdentityManager.SetObjectID(ref identityObject);

        NetworkServer.SendToAll(new CreateItemObjectMessage { position = message.position, itemInfo = message.itemInfo, ID = identityObject.ID });
    }

    private void OnDestroyItemObject(NetworkConnectionToClient client, DestroyItemObjectMessage message)
    {
        IdentityManager.TryGetAtID(message.ID, out GameObject itemObject);
        if(itemObject != null)
        {
            Destroy(itemObject);
            NetworkServer.SendToAll(new DestroyItemObjectMessage { ID = message.ID });
        }
    }

    public PlayerNetwork GetPlayer(uint targetNetID)
    {
        var s = netPlayers.Where(x => x.player.GetComponent<PlayerNetwork>().netId == targetNetID).FirstOrDefault().player.GetComponent<PlayerNetwork>();
        return s;
    }
    public PlayerNetwork GetPlayer(NetworkConnectionToClient conn)
    {
        return netPlayers.Where(x => x.conn == conn).FirstOrDefault().player.GetComponent<PlayerNetwork>();
    }
    public static void AddPlayer(NetworkConnectionToClient conn, GameObject player)
    {
        instance.netPlayers.Add(new NetPlayerCase()
        {
            conn = conn,
            player = player
        });
    }
    private void OnReadyToSync(NetworkConnectionToClient client, ReadyToSyncMessage message)
    {
        print($"[SERVER] клиент {client.connectionId} готов к синхронизации");
        print($"[SERVER] инициализирую объект игрока клиента {client.connectionId} ");
        gameManager.ServerSetPlayer(client);
        print("[SERVER] отправл€ю информацию о инициализации игрока клиента");
        client.Send(new InitPlayerMessage { });
        print("[SERVER] отправл€ю информацию о предметах на сервере");
        client.Send(new InitItemObjects { items = ItemObjectSyncer.GetItemObjectData() });
        print("[SERVER] отправл€ю информацию о состо€ни€х объектов на сервере");
        client.Send(new StatetableMessage { statetableObjects = StatetableManager.GetStatetableObjects() });

    }
    private void OnClaimSync(NetworkConnectionToClient client, ClaimSyncMessage message)
    {
        print($"[SERVER] клиент {client.connectionId} закончил синхронизацию");
    }
}
[Serializable]
public struct NetPlayerCase
{
    public NetworkConnectionToClient conn;
    public GameObject player;
}
