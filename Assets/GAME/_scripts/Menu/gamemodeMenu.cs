using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gamemodeMenu : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string[] descriptions;
    [SerializeField] private Text text;

    public void Change(int i)
    {
        text.text = descriptions[i];
    }
}
