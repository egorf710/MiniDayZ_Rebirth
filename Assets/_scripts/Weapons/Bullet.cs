
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private const float delta = 60f;
    private const float speed = 1.5294f;
    public void Init(Vector3 pos, float distance)
    {
        GetComponent<TrailRenderer>().time = distance / speed;
        StartCoroutine(IEUpdate(pos));
    }
    VulnerableObject target;
    int damage;
    public void Init(Vector3 pos, VulnerableObject zedBase, int damage, float distance)
    {
        GetComponent<TrailRenderer>().time = distance / speed / 2;
        this.damage = damage;
        this.target = zedBase;
        StartCoroutine(IEUpdate(pos));
    }
    IEnumerator IEUpdate(Vector3 pos)
    {
        while (transform.position != pos)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos, (delta * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }
        if(target != null)
        {
            target.TakeDamage(damage);
        }
        Destroy(gameObject);
        yield return null;
    }    
}
