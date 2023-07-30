using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCase : MonoBehaviour
{
    public string itemName;
    [SerializeField] private GameObject selected, pressed;
    [SerializeField] private Sprite unlockSprite;
    [SerializeField] private Image icon;
    public bool select;
    public bool press;
    [TextArea(3, 5)]
    public string description;
    private ItemCaseManager itemCaseManager;
    public bool open;

    private void Start()
    {
        itemCaseManager = FindObjectOfType<ItemCaseManager>();
    }

    public void Press()
    {
        if (itemCaseManager.UpdatePress(transform.parent.GetSiblingIndex()))
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
