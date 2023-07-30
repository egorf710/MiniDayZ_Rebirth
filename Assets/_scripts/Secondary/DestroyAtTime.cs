using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtTime : MonoBehaviour
{
    public int TimeDestroy;
    private void Start()
    {
        Destroy(gameObject, TimeDestroy);
    }
}
