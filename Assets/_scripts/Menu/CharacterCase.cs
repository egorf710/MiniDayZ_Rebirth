using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCase : MonoBehaviour
{
    public string charName;
    [SerializeField] private GameObject selected, pressed;
    public bool select;
    public bool press;
    [TextArea(3, 5)]
    public string description;
    private CharacterManager characterManager;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite unlockSprite;
    public bool open;

    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
    }

    public void Press()
    {
        if (characterManager.UpdatePress(transform.GetSiblingIndex()))
        {
            press = true;
            selected.SetActive(select);
            pressed.SetActive(true);
        }
    }
    public void Select()
    {
        press = false;
        select = true;
        selected.SetActive(true);
        pressed.SetActive(false);
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
