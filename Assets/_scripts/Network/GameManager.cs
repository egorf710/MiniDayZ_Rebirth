using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System;
using System.Linq;
using JetBrains.Annotations;


public class GameManager : NetworkManager
{
    public bool vanishMode;

    private new void Start()
    {
        base.Start();
    }
    public void StartGame()
    {
        var manager = this as NetworkManager;

        if (!Session.dedicatedServerMode)
        {
            manager.StartHost();
        }
        else
        {
            manager.StartServer();
        }
    }

    public void Connect(string addres)
    {
        networkAddress = addres;

        StartClient();

    }
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        StartCoroutine(StartSync());
    }
    IEnumerator StartSync()
    {
        yield return new WaitForSeconds(1f);
        print("[CLEINT] сцена загружена, отправл€ю сообщение на сервер в виде готовности к синхронизации");
        NetworkClient.Send(new ReadyToSyncMessage { });
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {

    }

    public void ServerSetPlayer(NetworkConnectionToClient conn)
    {

        Transform startPos = GameObject.Find("SPAWNPOINT").transform;
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        player.name = $"Player [connId={conn.connectionId}]";


        NetworkServer.AddPlayerForConnection(conn, player);//—павним игрока у клиентов
        player.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);//ѕозвол€ем клиенту управл€ем своим игроком на сервере (использовать [Comand] на объекте своего игрока)
        //”ведомл€ем все компоненты о том что подключилс€ новый игрок, чтобы настроить их работу с новым игроком
        Initializer.UpdateNetState();

        ServerManager.AddPlayer(conn, player);

        print($"[SERVER] отправл€ю клиенту {conn.connectionId} сообщение, чтобы он завершил локальную синхронизацию на своей стороне");
        conn.Send(new InitPlayerMessage { });
    }

    public void TeleportToSpawn()
    {
        Transform startPos = GameObject.Find("SPAWNPOINT").transform;
        NetworkClient.localPlayer.transform.position = startPos.position;
    }


}