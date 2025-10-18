using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    public ServerManager ServerManager;
    public List<PlayerNetwork> playerNetworks = new List<PlayerNetwork>();
    public wData GetData()
    {
        wData data = new wData();

        return data;
    }
    public void SetData(wData data)
    {
    }
}
public class wData
{
    public List<PlayerData> PlayerData;

}
