
using Mirror;
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
    GameObject target;
    int damage;
    public void Init(Vector3 pos, GameObject zedBase, int damage, float distance)
    {
        GetComponent<TrailRenderer>().time = distance / speed / 2;
        this.damage = damage;
        this.target = zedBase.gameObject;
        print("my target is " + target.name);
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
            if(target.TryGetComponent<PlayerNetwork>(out PlayerNetwork playerNetwork))
            {
                print("i try damage player");
                NetworkClient.localPlayer.GetComponent<PlayerNetwork>().TakeDamageTo(damage, playerNetwork.netId);
            }
            if (target.TryGetComponent(out VulnerableObject component))
            {
                component.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
        yield return null;
    }    
}
