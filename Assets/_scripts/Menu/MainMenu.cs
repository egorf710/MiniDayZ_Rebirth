using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public menu[] menus;
    public GameObject currentMenu;
    private int i;

    public AudioSource mySource;

    public void Open(int i)
    {
        currentMenu.GetComponent<menu>().Disable();
        this.i = i;
    }
    public void OpenMenu()
    {
        currentMenu = menus[i].gameObject;
        currentMenu.SetActive(true);
    }
}
