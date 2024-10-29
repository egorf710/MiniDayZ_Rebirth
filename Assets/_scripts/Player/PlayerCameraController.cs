using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour, Initable
{
    [SerializeField] private float smooth;
    [SerializeField] private Vector3 offset;
    [SerializeField] public Transform target;

    private void Update()
    {
        if(target != null)
        {
            transform.position = (Vector3)Vector3.MoveTowards(transform.position, target.position + offset, smooth);
        }
    }

    public void Init(Transform player)
    {
        target = player;
    }

    public void NetUpdate()
    {

    }
}
