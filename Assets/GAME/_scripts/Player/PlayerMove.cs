using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerAnimator playerAnimator;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Получаем ввод (WASD / стрелки)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Нормализация, чтобы не было ускорения по диагонали
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Двигаем персонажа через Rigidbody2D
        rb.velocity = movement * moveSpeed;

        playerAnimator.speed = moveSpeed;
        playerAnimator.animationDir = movement;
    }
}