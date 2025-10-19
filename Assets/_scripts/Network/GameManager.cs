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
        print("[CLEINT] ����� ���������, ��������� ��������� �� ������ � ���� ���������� � �������������");
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


        NetworkServer.AddPlayerForConnection(conn, player);//������� ������ � ��������
        player.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);//��������� ������� ��������� ����� ������� �� ������� (������������ [Comand] �� ������� ������ ������)
        //���������� ��� ���������� � ��� ��� ����������� ����� �����, ����� ��������� �� ������ � ����� �������
        Initializer.UpdateNetState();

        ServerManager.AddPlayer(conn, player);

        print($"[SERVER] ��������� ������� {conn.connectionId} ���������, ����� �� �������� ��������� ������������� �� ����� �������");
        conn.Send(new InitPlayerMessage { });
    }

    public void TeleportToSpawn()
    {
        Transform startPos = GameObject.Find("SPAWNPOINT").transform;
        NetworkClient.localPlayer.transform.position = startPos.position;
    }


}