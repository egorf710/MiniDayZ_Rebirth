using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetupCanvas : MonoBehaviour
{
    public TMP_Text logText;

    public static SetupCanvas instance;
    private void Awake()
    {
        instance = this;
    }
    public static void SetText(string text)
    {
        if(instance == null) {return; }
        instance.logText.text = text;
    }
    public static void Destroy()
    {
        Destroy(instance.gameObject);
    }
}
