using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZedAimOutline : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Image myImage;
    [SerializeField] private RectTransform myTransform;
    [SerializeField] private Camera myCamera;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Sprite[] aimImages;
    private void Start()
    {
        if (myImage == null)
        {
            myImage = GetComponent<Image>();
        }
        if(myCamera == null)
        {
            myCamera = Camera.main;
        }
        if(myImage != null)
        {
            myTransform = myImage.rectTransform;
        }

        offset = new Vector2(0, 0.4f);
    }
    public void SetTarget(Transform target)
    {
        myImage.enabled = target != null;
        this.target = target;
        if (target != null)
        {
            StartCoroutine(OutlineUpdate());
        }
    }
    public void HideAndStop()
    {
        myImage.enabled = false;
        target = null;
    }

    private IEnumerator OutlineUpdate()
    {
        while(target != null)
        {
            Vector2 vector = myCamera.WorldToScreenPoint((Vector2)target.position + offset);
            vector = new Vector2(Mathf.Clamp(vector.x, 0, canvas.anchoredPosition.x * 2), Mathf.Clamp(vector.y, 0, canvas.anchoredPosition.y * 2));
            myTransform.position = vector;
            yield return new WaitForSeconds(0.025f);
        }
        myImage.enabled = false;
    }
    public void DrawAimImageByAimRange(float aimRange)
    {
        if(aimRange >= 8)
        {
            myImage.sprite = aimImages[4];
            return;
        }
        else if(aimRange >= 6)
        {
            myImage.sprite = aimImages[3];
            return;
        }
        else if (aimRange >= 4)
        {
            myImage.sprite = aimImages[2];
            return;
        }
        else if (aimRange >= 2)
        {
            myImage.sprite = aimImages[1];
            return;
        }
        else
        {
            myImage.sprite = aimImages[0];
            return;
        }
    }
}
