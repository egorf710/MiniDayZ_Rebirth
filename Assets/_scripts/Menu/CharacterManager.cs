using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterCase currentCharCase;
    [SerializeField] private CharacterCase selectedCharCase;
    [SerializeField] private Transform parent;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject selectButton;
    public List<int> opened;
    private void OnEnable()
    {
        foreach (var item in opened)
        {
            parent.GetChild(item).GetComponent<CharacterCase>().Open();
        }
    }
    public bool UpdatePress(int i)
    {
        CharacterCase CharCase = parent.GetChild(i).GetComponent<CharacterCase>();
        selectButton.SetActive(CharCase.open);
        if(CharCase == currentCharCase) { return false; }
        currentCharCase.UnPressed();
        currentCharCase = CharCase;
        descriptionText.text = currentCharCase.description;
        return true;
    }
    public void Select()
    {
        selectedCharCase.UnPressed();
        selectedCharCase.select = false;
        currentCharCase.UnPressed();
        currentCharCase.select = true;
        currentCharCase.Select();
        selectedCharCase = currentCharCase;
    }
}
