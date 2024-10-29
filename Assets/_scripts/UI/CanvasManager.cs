using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour, Initable
{
    [SerializeField] private Button interactButton;
    [SerializeField] private Sprite FellingTreeSprite;
    private Sprite defaultInteractSprite;
    public static CanvasManager Instance;
    public PlayerInteract playerInteract;


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
    [SerializeField] public ZedAimOutline aimOutline;

    private void Awake()
    {
        Instance = this;
        //Init(FindObjectOfType<PlayerMove>().transform);
        defaultInteractSprite = interactButton.GetComponent<Image>().sprite;
    }
    public void Init(Transform player)
    {
        playerInteract = player.transform.GetChild(1).GetComponent<PlayerInteract>();
        player.transform.GetComponent<PlayerCharacteristics>().InitUI
            (healthField, waterField, foodField, heatField, 
            healthImage, bleedingImage, speedUpImage, sickImage);
        interactButton.onClick.AddListener(Interact);
        FindObjectOfType<WeaponManager>().Init(player);
        FindObjectOfType<InventoryManager>().Init(player);

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

    public void NetUpdate()
    {

    }
}
