using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._scripts.Interfaces;
using Microsoft.VisualBasic;
using Mirror;
using UnityEngine;

public struct StatetableObject
{
    public uint netId;
    public int state;
}

public class StatetableManager : MonoBehaviour
{
    public static StatetableManager instance;

    private void Awake()
    {
        instance = this;
    }
    public static List<StatetableObject> GetStatetableObjects()
    {
        return instance.iGetStatetableObjects();
    }
    public List<StatetableObject> iGetStatetableObjects()
    {
        List<StatetableObject> res = new List<StatetableObject>();
        foreach (var item in NetworkServer.spawned)
        {
            NetworkServer.spawned.TryGetValue(item.Key, out var identity);
            if (identity != null)
            {
                identity.gameObject.TryGetComponent(out Statetable component);
                if (component != null)
                {
                    res.Add(new StatetableObject() { netId = item.Key, state = component.GetStatetable() });
                }
            }
        }

        return res;
    }
}
