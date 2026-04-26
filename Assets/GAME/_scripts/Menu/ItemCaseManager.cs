using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCaseManager : MonoBehaviour
{
    [SerializeField] private ItemCase currentGunCase;
    [SerializeField] private ItemCase selectedGunCase;
    [SerializeField] private Transform parent;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private List<int> opened;
    private void OnEnable()
    {
        foreach (var item in opened)
        {
            parent.GetChild(item).GetChild(0).GetComponent<ItemCase>().Open();
        }
    }
    public bool UpdatePress(int i)
    {
        ItemCase CharCase = parent.GetChild(i).GetChild(0).GetComponent<ItemCase>();
        if (currentGunCase != null)
        {
            if (CharCase == currentGunCase) { return false; }
            currentGunCase.UnPressed();
            currentGunCase = CharCase;
            descriptionText.text = currentGunCase.description;
        }
        else
        {
            currentGunCase = CharCase;
            descriptionText.text = currentGunCase.description;
        }
        return true;
    }
}
