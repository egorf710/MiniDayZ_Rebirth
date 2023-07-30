using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuMessager : MonoBehaviour
{
    [SerializeField] private Text TextPrefab;
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
    }
    public static void Mess(string text, Color color)
    {
        Text textObj = Instantiate(Instance.TextPrefab.gameObject, Instance.transform).GetComponent<Text>();
        textObj.text = text;
        textObj.color = color;
    }
    public static void Mess(string text, Color color, Vector2 position)
    {
        Text textObj = Instantiate(Instance.TextPrefab.gameObject, Instance.transform).GetComponent<Text>();
        textObj.text = text;
        textObj.transform.localPosition = position;
    }
}
