using Assets._scripts.World;
using Microsoft.VisualBasic;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerManager : NetworkBehaviour, Initable
{
    [SerializeField] private TargetManager targetManager;
    public static ServerManager instance;
    public List<NetPlayerCase> netPlayers = new List<NetPlayerCase>();
    public PlayerNetwork playerNetwork;
    private void Awake()
    {
        instance = this;
    }
    public void Init(Transform player)
    {
        playerNetwork = player.GetComponent<PlayerNetwork>();
    }

    //[Command]
    public void TakeDamage(uint targetNetID, int damage)
    {
        TakeDamageSM(targetNetID, damage);
    }
    public PlayerNetwork GetPlayer(uint targetNetID)
    {
        //print("i find: " + targetNetID);
        var s = netPlayers.Where(x => x.player.GetComponent<PlayerNetwork>().netId == targetNetID).FirstOrDefault().player.GetComponent<PlayerNetwork>();
        //print("i finded s: " + s.netId);
        return s;
    }
    public PlayerNetwork GetPlayer(NetworkConnectionToClient conn)
    {
        return netPlayers.Where(x => x.conn == conn).FirstOrDefault().player.GetComponent<PlayerNetwork>();
    }
    //[ClientRpc]
    private void TakeDamageSM(uint targetNetID, int damage)
    {
        //if (netIdentity.isServer) { return; }
        targetManager.targetsBases.Find(x => x.getNetID() == targetNetID).TakeDamage(damage);
    }
    //[Command]
    public void DropLoot(DropLootMassage dropLootMassage)
    {
        DropLootSM(dropLootMassage);
    }
    //[ClientRpc]
    private void DropLootSM(DropLootMassage dropLootMassage)
    {
        Vector2 pos = dropLootMassage.pos;
        ItemLootData[] itemLoots = dropLootMassage.loot;
        foreach (var loot in itemLoots)
        {
            InventoryManager.SpawnLoot(itemLoots, pos);
        }
    }
    public void TakePlayerDamage(int netID, int damage)
    {
        (netPlayers.Where(x => x.player.GetComponent<PlayerNetwork>().netId == netID).FirstOrDefault().player.GetComponent<PlayerNetwork>() as AliveTarget).TakeDamage(damage); 
    }
    public void NetUpdate()
    {
    }
    public static PlayerNetwork GetMyPlayer() => instance.playerNetwork;

    public static void DestroyItemObjectAtID(int ID)
    {
        instance.playerNetwork.CMDDestroyItemObjectAtID(ID);
    }

    public static void TeleportToSpawn(int secondRestart)
    {
        instance.StartCoroutine(instance.IEStartFunAtTime(secondRestart));
    }
    private IEnumerator IEStartFunAtTime(float time)
    {

        yield return new WaitForSeconds(time);
        var GM = FindObjectOfType<GameManager>();
        GM.TeleportToSpawn();
        if (!GM.vanishMode)
        {

            SetActivePlayer(true);
            CanvasManager.SetActiveDeathPanel(false);
        }
    }

    public static void SetActivePlayer(bool b)
    {
        instance.ISetActivePlayer(b);
    }
    private void ISetActivePlayer(bool b)
    {
        playerNetwork.SetActivePlayerSkin(b);
    }
}
[Serializable]
public struct NetPlayerCase
{
    public NetworkConnectionToClient conn;
    public GameObject player;
}
