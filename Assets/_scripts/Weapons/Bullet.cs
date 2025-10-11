
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1.5294f;
    public void Init(Vector3 pos, int damage)
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

    }
    public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerNetwork>(out PlayerNetwork playerNetwork))
        {
            //print("i try damage player");
            if(NetworkClient.localPlayer.netId == playerNetwork.netId) { return; }
            NetworkClient.localPlayer.GetComponent<PlayerNetwork>().TakeDamageTo(damage, playerNetwork.netId, 1);
        }
        if (collision.TryGetComponent(out VulnerableObject component))
        {
          
            component.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerNetwork>(out PlayerNetwork playerNetwork))
        {
            //print("i try damage player");
            if (NetworkClient.localPlayer.netId == playerNetwork.netId) { return; }
            NetworkClient.localPlayer.GetComponent<PlayerNetwork>().TakeDamageTo(damage, playerNetwork.netId, 1);
        }
        if (collision.collider.TryGetComponent(out VulnerableObject component))
        {
            component.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
