using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{
    public Transform player;
    public static Initializer instance;
    public static bool canInit = false;
    private void Awake()
    {
        instance = this;
    }
    public static void Init(Transform player)
    {
        instance.player = player;
        if (player.GetComponent<PlayerNetwork>().isLocalPlayer)
        {
            List<Initable> initables = FindObjectsOfType<MonoBehaviour>(true).OfType<Initable>().ToList();
            foreach (var initable in initables)
            {
                initable.Init(player);
            }
            canInit = true;
        }
    }
    public static void UpdateNetState()
    {
        List<Initable> initables = FindObjectsOfType<MonoBehaviour>(true).OfType<Initable>().ToList();
        foreach (var initable in initables)
        {
            initable.NetUpdate();
        }
        print("init");
    }
}
public interface Initable
{
    public void Init(Transform player);
    public void NetUpdate();
    public bool CanInit()
    {
        return Initializer.canInit;
    }
}
public interface SyncGameObject
{
    public bool AlsoSaveMe();
}