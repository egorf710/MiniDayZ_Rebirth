using Assets._scripts.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TargetManager : MonoBehaviour
{
    [SerializeField] public List<AliveTarget> targetsBases = new List<AliveTarget>();
    [SerializeField] private AliveTarget myplayer;
    [SerializeField] private Transform mytransformplayer;
    public static TargetManager Instance;

    private void Awake()
    {
        Instance = this;

        myplayer = mytransformplayer.GetComponent<PlayerNetwork>();



        PlayerNetwork[] targets = FindObjectsOfType<PlayerNetwork>();

        foreach (var target in targets)
        {
            AddTarget(target);
        }
    }

    #region Target

    public static AliveTarget[] GetTargetsAtPoint(Vector3 point, float radius, bool onlyNoticedPlayers = true)
    {
        return Instance.InstanceGetTargetsAtPoint(point, radius, onlyNoticedPlayers);
    }
    private AliveTarget[] InstanceGetTargetsAtPoint(Vector3 point, float radius, bool onlyNoticedPlayers = true)
    {
        AliveTarget[] _playerBases = null;
        if (onlyNoticedPlayers)
        {
            _playerBases = targetsBases.Where(x => GetDistance(mytransformplayer.position, x.getTransform().position) < 30).ToArray();
        }
        else
        {
            _playerBases = targetsBases.ToArray();
        }
        return _playerBases.Where(v2 => GetDistance(point, v2.getTransform().position) <= radius).ToArray();
    }
    public static void SetTargets(AliveTarget[] playerBase)
    {
        for (int i = 0; i < playerBase.Length; i++)
        {
            if (Instance.myplayer == playerBase[i]) { continue; }
            Instance.targetsBases.Add(playerBase[i]);
        }
    }
    public static void AddTarget(AliveTarget playerBase)
    {
        if (Instance.myplayer == playerBase || Instance.targetsBases.Contains(playerBase)) { return; }
        Instance.targetsBases.Add(playerBase);
        //print("add " + playerBase.getTransform().name);
    }
    public static void RemoveTarget(AliveTarget playerBase)
    {
        if (Instance.myplayer == playerBase) { return; }
        Instance.targetsBases.Remove(playerBase);
        //print("remove " + playerBase.getTransform().name);
    }

    #endregion

    private float GetDistance(Vector2 v1, Vector3 v2)
    {
        return Vector2.Distance(v1, v2);
    }
}
