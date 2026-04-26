using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunCase : MonoBehaviour
{
    public string gunName;
    [SerializeField] private GameObject selected, pressed;
    [SerializeField] private Sprite unlockSprite;
    [SerializeField] private Image icon;
    public bool select;
    public bool press;
    [TextArea(3, 5)]
    public string description;
    private GunCaseManager gunCaseManager;
    public bool open;

    private void Start()
    {
        gunCaseManager = FindObjectOfType<GunCaseManager>();
    }

    public void Press()
    {
        if (gunCaseManager.UpdatePress(transform.parent.GetSiblingIndex()))
        {
            press = true;
            pressed.SetActive(true);
        }
    }
    public void UnPressed()
    {
        press = false;
        selected.SetActive(false);
        pressed.SetActive(false);
    }
    public void Open()
    {
        open = true;
        icon.sprite = unlockSprite;
    }
}
