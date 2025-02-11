using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform up, down;
    [SerializeField] private Transform upPos, downPos, logoPos;
    [SerializeField] private Image logo;
    [SerializeField] private GameObject mainButtons;

    [SerializeField] private Vector3 _upPos, _downPos, _logoPos;
    [SerializeField] private Vector3 _upPos2, _downPos2, _logoPos2;

    private void Start()
    {
        _upPos = up.transform.position;
        _downPos = down.transform.position;
        _logoPos = logo.transform.position;
        _upPos2 = upPos.transform.position;
        _downPos2 = downPos.transform.position;
        _logoPos2 = logoPos.transform.position;
        StartCoroutine(IEMoveLogo());
        StartCoroutine(IEMoveBG());
    }

    IEnumerator IEMoveLogo()
    {
        yield return new WaitForSeconds(2);
        FindObjectOfType<MainMenu>().mySource.enabled = true;
        while(logo.transform.position != logoPos.position)
        {
            logo.transform.position = Vector2.MoveTowards(logo.transform.position, logoPos.transform.position, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        yield return null;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(_IEMoveLogo());
            StartCoroutine(_IEMoveBG());
        }
    }
    IEnumerator _IEMoveLogo()
    {
        FindObjectOfType<MainMenu>().mySource.enabled = true;
        while (logo.transform.position != _logoPos)
        {
            logo.transform.position = Vector2.MoveTowards(logo.transform.position, _logoPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        yield return null;
    }
    IEnumerator _IEMoveBG()
    {
        while (up.transform.position != _upPos || down.transform.position != _downPos)
        {
            up.transform.position = Vector2.MoveTowards(up.transform.position, _upPos, speed * Time.deltaTime);
            down.transform.position = Vector2.MoveTowards(down.transform.position, _downPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        mainButtons.SetActive(true);
        mainButtons.GetComponent<menu>().enabled = true;
        yield return null;
    }
    IEnumerator IEMoveBG()
    {
        yield return new WaitForSeconds(2);
        while ((int)up.transform.position.y != (int)_upPos2.y || (int)down.transform.position.y != (int)_downPos2.y)
        {

            up.transform.position = Vector2.MoveTowards(up.transform.position, upPos.transform.position, speed * Time.deltaTime);
            down.transform.position = Vector2.MoveTowards(down.transform.position, downPos.transform.position, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        mainButtons.SetActive(true);
        mainButtons.GetComponent<menu>().enabled = true;
        yield return null;
    }

}
