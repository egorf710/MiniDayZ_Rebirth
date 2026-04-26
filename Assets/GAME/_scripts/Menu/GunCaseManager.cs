using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunCaseManager : MonoBehaviour
{
    [SerializeField] private GunCase currentGunCase;
    [SerializeField] private GunCase selectedGunCase;
    [SerializeField] private Transform parent;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private List<int> opened;
    private void OnEnable()
    {
        foreach (var item in opened)
        {
            parent.GetChild(item).GetChild(0).GetComponent<GunCase>().Open();
        }
    }
    public bool UpdatePress(int i)
    {
        GunCase CharCase = parent.GetChild(i).GetChild(0).GetComponent<GunCase>();
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
