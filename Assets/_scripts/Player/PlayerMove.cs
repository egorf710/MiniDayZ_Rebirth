using Microsoft.VisualBasic;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public float base_movement_speed = 1f;
    [SerializeField] private float movement_speed;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private bool IsGoToPoint;
    private void Update()
    {
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movement_speed = Mathf.Clamp(moveDirection.magnitude, 0f, 1f);
        moveDirection.Normalize();
        if (!IsGoToPoint)
        {
            playerAnimator.speed = moveDirection.magnitude;
        }
        if(moveDirection.magnitude > 0)
        {
            playerAnimator.animationDir = moveDirection;
        }
    }
    private void FixedUpdate()
    {
        if (IsGoToPoint) { return; }
        rb.velocity = moveDirection * base_movement_speed;
    }
    public void GoToPoint(Vector2 point)
    {
        IsGoToPoint = false;
        moveDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
        StopCoroutine(IEGoTo(point));
        StartCoroutine(IEGoTo(point));
    }
    private IEnumerator IEGoTo(Vector2 point)
    {
        IsGoToPoint = true;
        Vector2 dir = (Vector3)point - transform.position;
        while(moveDirection.magnitude <= 0.5f)
        {
            if (Vector2.Distance(transform.position, point) > 0.5f)
            {
                rb.velocity = dir.normalized * base_movement_speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
                IsGoToPoint = false;
                playerAnimator.speed = 0;
                break;
            }
            playerAnimator.speed = rb.velocity.normalized.magnitude;
            playerAnimator.animationDir = dir.normalized;
            yield return new WaitForFixedUpdate();
        }
        playerAnimator.speed = 0;
        IsGoToPoint = false;
    }
}
