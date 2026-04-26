using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZedAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Vector2 lastDir;

    public void OnEnable()
    {
        anim.enabled = true;
    }
    public void OnDisable()
    {
        anim.enabled = false;
    }

    public void AnimMove(Vector2 direction, bool walk = false)
    {
        anim.SetBool("walk", walk);
        if (direction.magnitude > 0)
        {
            lastDir = new Vector2(direction.x, direction.y);
        }

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            lastDir = new Vector2(direction.x, 0);
        }
        else
        {
            lastDir = new Vector2(0, direction.y);
        }

        anim.SetFloat("x", lastDir.x);
        anim.SetFloat("y", lastDir.y);
    }
    public void AnimIdle()
    {
        if (Mathf.Abs(lastDir.x) >= Mathf.Abs(lastDir.y))
        {
            if (lastDir.x >= 0)
            {
                lastDir = new Vector2(0.05f, 0);
            }
            else
            {
                lastDir = new Vector2(-0.05f, 0);
            }
        }
        else
        {
            if (lastDir.y >= 0)
            {
                lastDir = new Vector2(0, 0.05f);
            }
            else
            {
                lastDir = new Vector2(0, -0.05f);
            }
        }
        anim.SetFloat("x", lastDir.x);
        anim.SetFloat("y", lastDir.y);
    }
    public void AnimDie()
    {
        anim.SetTrigger("die1");
    }
    public void Dest()
    {
        Destroy(gameObject);
    }
}
