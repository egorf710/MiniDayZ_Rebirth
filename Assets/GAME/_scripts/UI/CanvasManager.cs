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


    [Space]
    [Header("Initer")]
    [SerializeField] private Slider healthField;
    [SerializeField] private Slider waterField;
    [SerializeField] private Slider foodField;
    [SerializeField] private Slider heatField;
    [Space]
    [SerializeField] private Image healthImage;
    [SerializeField] private Image bleedingImage;
    [SerializeField] private Image speedUpImage;
    [SerializeField] private Image sickImage;
    [SerializeField] public GameObject deathPanel;

    private void Awake()
    {
        Instance = this;
        defaultInteractSprite = interactButton.GetComponent<Image>().sprite;
    }
    public void Init(Transform player)
    {
        interactButton.onClick.AddListener(Interact);
        FindObjectOfType<InventoryManager>().Init(player);

    }
    public static void SetActiveInteractButton(bool b)
    {
        Instance.interactButton.GetComponent<Image>().sprite = Instance.defaultInteractSprite;
        Instance.interactButton.GetComponent<Image>().color = new Color(1, 1, 1, b ? 1 : 0.6f);
    }
    public static void SetActiveDeathPanel(bool b)
    {
        Instance.deathPanel.SetActive(b);
    }
    private void Interact()
    {

    }

    public static void SetActiveFellingTreeButton(bool v)
    {
        Instance.interactButton.GetComponent<Image>().sprite = v ? Instance.FellingTreeSprite : Instance.defaultInteractSprite;
    }
}
