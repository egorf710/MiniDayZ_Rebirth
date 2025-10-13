using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStart : MonoBehaviour
{
    public GameManager gameManager;
    void Start()
    {
        Session.dedicatedServerMode = true;
        gameManager.Start();
        gameManager.StartGame();
    }
}
