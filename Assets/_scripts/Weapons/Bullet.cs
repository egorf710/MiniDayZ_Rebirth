
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1.5294f;
    [SerializeField] private bool server;
    [SerializeField] private uint admin;
    public void Init(Vector3 pos, int damage, bool _server, uint _admin)
    {
        var rb = GetComponent<Rigidbody2D>();
        // Обнуляем текущую скорость
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector2 direction = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        this.damage = damage;

        // Применяем силу
        rb.AddForce(direction * speed, ForceMode2D.Impulse);

        server = _server;
        admin = _admin;
    }
    public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent<PlayerNetwork>(out PlayerNetwork playerNetwork))
        {
            if (playerNetwork.netId == admin) { return; }
            if (server)
            {
                playerNetwork.CMDTakeDamage(damage, 1);
            }
        }
        else if (collision.TryGetComponent(out VulnerableObject component))
        {
            component.TakeDamage(damage);
        }


        Destroy(gameObject);
    }
}
