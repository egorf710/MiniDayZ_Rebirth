using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AimLines : MonoBehaviour
{
    [SerializeField] private Sprite[] color_Sprites;
    [SerializeField] private SpriteRenderer[] lines;
    [SerializeField] private List<float> color_Indexes = new List<float>();
    [SerializeField] AnimationCurve alphaCurve;
    [SerializeField] float min, max, k;

    public void Set(float _min, float _max)
    {
        min = _min;
        max = _max;

        k = 1 / (max - min);

        float colorUnit = max / 5;

        for (int i = 0; i < color_Indexes.Count; i++)
        {
            color_Indexes[i] = colorUnit * i;
        }
    }
    public void DrawLines(float aimrange, bool lerp)
    {
        if (!lerp)
        {
            lines[0].transform.localRotation = Quaternion.Euler(0, 0, -aimrange);
            lines[1].transform.localRotation = Quaternion.Euler(0, 0, aimrange);
        }
        else
        {
            lines[0].transform.localRotation = Quaternion.Lerp(lines[0].transform.localRotation, Quaternion.Euler(0, 0, -aimrange), 0.1f);
            lines[1].transform.localRotation = Quaternion.Lerp(lines[1].transform.localRotation, Quaternion.Euler(0, 0, aimrange), 0.1f);
        }


        int colorIndex = 0;
        foreach (var item in color_Indexes)
        {
            if(aimrange > item) { colorIndex++; }
        }
        if (colorIndex >= color_Indexes.Count) { colorIndex = color_Indexes.Count - 1; }

        lines[0].sprite = color_Sprites[colorIndex];
        lines[1].sprite = color_Sprites[colorIndex];

        //lines[0].color = new Color(1, 1, 1, alphaCurve.Evaluate(aimrange * k + 0.001f));
        //lines[1].color = new Color(1, 1, 1, alphaCurve.Evaluate(aimrange * k + 0.001f));

        lines[0].transform.localPosition = new Vector2(0.1f + alphaCurve.Evaluate(aimrange * k / 2) / 5, 0.753f);
        lines[1].transform.localPosition = new Vector2((0.1f + alphaCurve.Evaluate(aimrange * k  / 2) / 5) * -1, 0.753f);
    }
}
