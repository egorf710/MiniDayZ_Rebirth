using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] MainMenu mainMenu;
    private void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
    }
    private void OnEnable()
    {
        StopAllCoroutines();

        StartCoroutine(IEShowHide(true));
    }
    public void Disable()
    {
        StopAllCoroutines();
        StartCoroutine(IEShowHide(false));
    }
    public void End()
    {
        mainMenu.OpenMenu();
    }
    IEnumerator IEShowHide(bool b)
    {
        List<Image> images = new List<Image>();
        List<Text> texts = new List<Text>();

        GetChildComponent(transform, ref images, ref texts);

        if (b)
        {
            for (int i = 0; i < (images.Count <= texts.Count ? texts.Count : images.Count); i++)
            {
                if (images.Count > i)
                {
                    images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0);
                }
                if (texts.Count > i)
                {
                    texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0);
                }
            }
        }


        while ((images.Count > 0 && (!b ? (images[0].color.a > 0) : (images[0].color.a < 1))) || (texts.Count > 0 && (!b ? (texts[0].color.a > 0) : (texts[0].color.a < 1))))
        {
            for (int i = 0; i < (images.Count <= texts.Count ? texts.Count : images.Count); i++)
            {
                if (images.Count > i)
                {
                    images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, images[i].color.a - (speed * (!b ? 1 : -1)) * Time.deltaTime);
                }
                if(texts.Count > i)
                {
                    texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, texts[i].color.a - (speed * (!b ? 1 : -1)) * Time.deltaTime);
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
        if (!b)
        {
            End();
            gameObject.SetActive(false);
        }
    }

    private void GetChildComponent(Transform parent, ref List<Image> images, ref List<Text> texts)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if(parent.GetChild(i).GetComponent<Image>() != null)
            {
                images.Add(parent.GetChild(i).GetComponent<Image>());
            }
            if (parent.GetChild(i).GetComponent<Text>() != null)
            {
                texts.Add(parent.GetChild(i).GetComponent<Text>());
            }
            else if(parent.GetChild(i).childCount > 0)
            {
                GetChildComponent(parent.GetChild(i), ref images, ref texts);
            }
        }
    }
}
