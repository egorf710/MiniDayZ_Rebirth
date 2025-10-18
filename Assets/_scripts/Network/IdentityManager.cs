using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdentityManager : MonoBehaviour
{
    public int nextID = 0;

    public static IdentityManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void SetObjectID(ref IdentityObject odject)
    {
        odject.ID = instance.nextID;
        instance.nextID++;
    }
    public static void TryGetAtID(int ID, out GameObject go)
    {
        var res = FindObjectsOfType<IdentityObject>().Where(x => x.ID == ID).FirstOrDefault();
        if(res == null)
        {
            go = null;
            Debug.LogError("UN SYNCED GAME OBJECT, try get: " +  ID + " object!!!");
        }
        else
        {
            go = res.gameObject;
        }
    }
}
