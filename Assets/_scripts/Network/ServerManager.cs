using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    [SerializeField] private TargetManager targetManager;
    public static ServerManager instance;
    public List<NetPlayerCase> netPlayers = new List<NetPlayerCase>();
    private void Awake()
    {
        instance = this;
    }
    public void Init(Transform player)
    {
        targetManager.Init(player);
    }
    //[Command]
    public void TakeDamage(uint targetNetID, int damage)
    {
        TakeDamageSM(targetNetID, damage);
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
}
[Serializable]
public struct NetPlayerCase
{
    public NetworkConnectionToClient conn;
    public GameObject player;
}
