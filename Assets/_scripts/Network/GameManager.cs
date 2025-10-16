using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System;

public class GameManager : NetworkManager
{
    public ServerManager serverManager;
    public WorldData worldData;
    public bool vanishMode;

    private new void Start()
    {
        base.Start();
        RegMessages();
    }
    public void StartGame()
    {
        var manager = this as NetworkManager;

        manager.StartHost();
    }
    public void Connect(string addres)
    {
        networkAddress = addres;

        StartClient();

    }
    private void RegMessages()
    {
        print("RegInitMsg");
        NetworkClient.RegisterHandler<InitPlayerMessage>(OnInitPlayerMessage);
        NetworkClient.RegisterHandler<HellowWorldMessage>(OnHellowWorldMessage);
    }

    ///CLIENT///CLIENT///CLIENT///CLIENT///CLIENT///CLIENT///CLIENT///CLIENT///
    ///
    public void OnInitPlayerMessage(InitPlayerMessage message)
    {
        print("OnInit: " + NetworkClient.localPlayer.transform.name);
        Initializer.Init(NetworkClient.localPlayer.transform);
        if(NetworkClient.localPlayer.isLocalPlayer && !NetworkClient.localPlayer.isServer)
        {
            NetworkClient.localPlayer.name = "Local Player";
        }
    }

    private void OnHellowWorldMessage(HellowWorldMessage message)
    {
        if (serverManager == null || worldData == null)
        {
            serverManager = FindObjectOfType<ServerManager>();
            worldData = FindObjectOfType<WorldData>();
        }
        print("load net data");
        if (serverManager.isClientOnly)
        {
            print("load net data!!!!");
            FindObjectOfType<WorldData>().SetData(message.worlddata);
        }
    }


    ///SERVER///SERVER///SERVER///SERVER///SERVER///SERVER///SERVER///SERVER///
    ///

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(serverManager == null || worldData == null)
        {
            serverManager = FindObjectOfType<ServerManager>();
            worldData = FindObjectOfType<WorldData>();
        }
        Transform startPos = GameObject.Find("SPAWNPOINT").transform;
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        if (player.GetComponent<NetworkIdentity>().isServer)
        {
            player.name = $"HOST - {playerPrefab.name} [connId={conn.connectionId}]";

        }
        NetworkServer.AddPlayerForConnection(conn, player);
        conn.Send(new InitPlayerMessage());

        player.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        

        serverManager.netPlayers.Add(new NetPlayerCase()
        {
            conn = conn,
            player = player
        });



        Initializer.UpdateNetState();
    
        if(vanishMode && NetworkServer.active && player.GetComponent<PlayerNetwork>().isLocalPlayer)
        {
            player.SetActive(false);
            print("server vanish active");
        }

    }

    public void TeleportToSpawn()
    {
        Transform startPos = GameObject.Find("SPAWNPOINT").transform;
        NetworkClient.localPlayer.transform.position = startPos.position;
    }
}
public struct HellowWorldMessage : NetworkMessage
{
    public wData worlddata;
}