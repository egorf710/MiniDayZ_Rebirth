using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Button interactButton;
    [SerializeField] private Sprite FellingTreeSprite;
    private Sprite defaultInteractSprite;
    public static CanvasManager Instance;
    public PlayerInteract playerInteract;
    private void Awake()
    {
        Instance = this;
        //Init(FindObjectOfType<PlayerMove>().transform);
        defaultInteractSprite = interactButton.GetComponent<Image>().sprite;
    }
    public void Init(Transform player)
    {
        playerInteract = player.transform.GetChild(1).GetComponent<PlayerInteract>();
        FindObjectOfType<InventoryManager>().Init(player);
        interactButton.onClick.AddListener(Interact);
    }
    public static void SetActiveInteractButton(bool b)
    {
        Instance.interactButton.GetComponent<Image>().sprite = Instance.defaultInteractSprite;
        Instance.interactButton.GetComponent<Image>().color = new Color(1, 1, 1, b ? 1 : 0.6f);
    }
    private void Interact()
    {
        playerInteract.Interact();
    }

    public static void SetActiveFellingTreeButton(bool v)
    {
        Instance.interactButton.GetComponent<Image>().sprite = v ? Instance.FellingTreeSprite : Instance.defaultInteractSprite;
    }
}
