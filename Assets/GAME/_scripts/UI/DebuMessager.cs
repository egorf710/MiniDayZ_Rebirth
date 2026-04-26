using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DebuMessager : MonoBehaviour
{
    [SerializeField] private Text TextPrefab;
    //[SerializeField] private float delta = 1f;
    public static DebuMessager Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Mess("test");
            return;
        }
        if (Input.GetKey(KeyCode.Escape) && Input.GetKeyDown(KeyCode.Space))
        {
            Mess("test", Color.yellow, new Vector2(0, 100));
        }
    }
    public static void Mess(string text)
    {
        Text textObj = Instantiate(Instance.TextPrefab.gameObject, Instance.transform).GetComponent<Text>();
        textObj.text = text;
        Instance.StartCoroutine(Instance.IEMove(textObj));
    }
    public static void Mess(string text, Color color)
    {
        Text textObj = Instantiate(Instance.TextPrefab.gameObject, Instance.transform).GetComponent<Text>();
        textObj.text = text;
        textObj.color = color;
        Instance.StartCoroutine(Instance.IEMove(textObj));
    }
    public static void Mess(string text, Color color, Vector2 position, bool objectPos = false)
    {
        Text textObj = Instantiate(Instance.TextPrefab.gameObject, Instance.transform).GetComponent<Text>();
        textObj.text = text;
        textObj.color = color;
        if (objectPos)
        {
            textObj.transform.position = Camera.main.WorldToScreenPoint(position);
        }
        else
        {
            textObj.transform.localPosition = position;
        }
        Instance.StartCoroutine(Instance.IEMove(textObj));
    }
    IEnumerator IEMove(Text text)
    {
        Vector2 startPos = Camera.main.ScreenToWorldPoint(text.transform.position);

        float end = text.transform.position.y + 69;
        Vector3 y = Vector3.zero;
        while (text != null && text.transform.position.y + 15 <= end && text.gameObject != null)
        {
            //15-69
            text.transform.position = Camera.main.WorldToScreenPoint(startPos) + y;
            y += Vector3.up;
            yield return new WaitForFixedUpdate();
        }
    }
}
