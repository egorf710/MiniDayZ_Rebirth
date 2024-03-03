using Assets._scripts.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public static ObjectSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject SpawnObject(GameObject gameObject, Vector3 pos, Quaternion rot)
    {
        if (gameObject.TryGetComponent<AliveTarget>(out AliveTarget aliveTarget))
        {
            TargetManager.AddTarget(aliveTarget);
        }

        return gameObject;
    }

    public void DestroyObject(GameObject gameObject, float time = 0f)
    {
        if (gameObject.TryGetComponent<AliveTarget>(out AliveTarget aliveTarget))
        {
            TargetManager.RemoveTarget(aliveTarget);
        }

        if (time != 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, time);
        }
    }
}
