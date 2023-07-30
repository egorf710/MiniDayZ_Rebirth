using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZedsManager : MonoBehaviour
{
    [SerializeField] private List<ZedBase> zedBases = new List<ZedBase>();
    public static ZedsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static ZedBase[] GetZedsAtPoint(Vector3 point, float radius, bool onlyEnabledZeds = true)
    {
        return Instance.InstanceGetZedsAtPoint(point, radius, onlyEnabledZeds);
    }
    private ZedBase[] InstanceGetZedsAtPoint(Vector3 point, float radius, bool onlyEnabledZeds = true)
    {
        ZedBase[] _zedBases = null;
        if (onlyEnabledZeds)
        {
            _zedBases = zedBases.Where(x => x.enable).ToArray();
        }
        else
        {
            _zedBases = zedBases.ToArray();
        }
        return _zedBases.Where(v2 => GetDistance(point, v2.transform.position) <= radius).ToArray();
    }
    private float GetDistance(Vector2 v1, Vector3 v2)
    {
        return Vector2.Distance(v1, v2);
    }

    public static void AddZed(ZedBase zedBase)
    {
        Instance.zedBases.Add(zedBase);
    }
    public static void RemoveZed(ZedBase zedBase)
    {
        Instance.zedBases.Remove(zedBase);
    }
}
