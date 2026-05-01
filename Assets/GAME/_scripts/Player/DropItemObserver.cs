using System.Collections;
using System.Collections.Generic;
using Assets.GAME._scripts.Fic;
using Assets.GAME._scripts.Inventory.Signals;
using UnityEngine;

public class DropItemObserver : MonoBehaviour
{
    [SerializeField] private Vector2 offset;
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private bool observe = false;
    private float timeToObserv;
    private void Update()
    {
        if(observe)
        {
            if(timeToObserv >= 1)
            {
                CheckDropedItems();
                timeToObserv = 0;
            }
            else
            {
                timeToObserv += Time.deltaTime;
            }
        }
    }
    void Start()
    {
        EventBus.Subscribe<Signal_UpdateDropItems>(UpdateDropItems);
    }
    private void UpdateDropItems(Signal_UpdateDropItems signal)
    {
        observe = signal.observ;

        CheckDropedItems();
    }
    private void CheckDropedItems()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position + (Vector3)offset, radius);

        List<ItemObject> itemObjects = new List<ItemObject>();

        foreach (var coll in colls)
        {
            if (coll.TryGetComponent<ItemObject>(out ItemObject io))
            {
                itemObjects.Add(io);
            }
        }

        EventBus.Invoke<Signal_RefreshDropPanel>(new Signal_RefreshDropPanel(itemObjects.ToArray()));
    }
}
