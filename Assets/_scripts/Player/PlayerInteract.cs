using Assets._scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemObject;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactDistance = 1;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private PlayerMove playerMove;
    private List<ItemObject> itemObjects = new List<ItemObject>();
    [SerializeField] private Tree aroundTree;
    [SerializeField] private InventorySlot handSlot;
    [SerializeField] private PlayerNetwork playerNetwork;
    private void Start()
    {
        playerNetwork = playerMove.GetComponent<PlayerNetwork>();
        if (!playerNetwork.isLocalPlayer) { return; }
        StartCoroutine(IEInteractChecker());
    }
    public void Interact()
    {
        Collider2D[] collsToInteract = Physics2D.OverlapCircleAll(transform.position, interactDistance);

        foreach (var coll in collsToInteract)
        {
            if(coll.TryGetComponent<Interactable>(out Interactable iteractableObject) && ((aroundTree != null && iteractableObject is Tree) || aroundTree == null))
            {
                if (iteractableObject.IsInteractable(out string message))
                {
                    iteractableObject.Interact();
                    break;
                }
                else
                {
                    //Debug.LogWarning($"Player doesn't interact with {coll.gameObject}, \nP.s " + message);
                }
            }
        }
    }
    private IEnumerator IEInteractChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            Collider2D[] collstoInteract = null;
            bool b = CanInteract(out collstoInteract);
            CanvasManager.SetActiveInteractButton(b);

            if (b)
            {
                foreach (var coll in collstoInteract)
                {
                    if (coll.GetComponent<Tree>() && coll.GetComponent<Tree>().IsActive())
                    {
                        CanvasManager.SetActiveFellingTreeButton(true);
                        aroundTree = coll.GetComponent<Tree>();
                        break;
                    }
                    else
                    {
                        CanvasManager.SetActiveFellingTreeButton(false);
                        aroundTree = null;
                    }
                }
            }

            if (GetAroundItems().Length != itemObjects.Count)
            {
                InventoryManager.RefreshDropPanel();
                itemObjects = GetAroundItems().ToList();
            }
        }
    }
    private bool CanInteract(out Collider2D[] collsToInteract)
    {
        collsToInteract = Physics2D.OverlapCircleAll(transform.position, interactDistance, layerMask);
        List<Collider2D> interactables = new List<Collider2D>();

        foreach (var item in collsToInteract)
        {
            if (item.GetComponent<Interactable>().IsActive())
            {
                interactables.Add(item);
            }
        }
        return interactables.Count > 0;
    }
    public ItemObject[] GetAroundItems()
    {
        Collider2D[] collsToInteract = Physics2D.OverlapCircleAll(transform.position, interactDistance);
        List<ItemObject> items = new List<ItemObject>();
        foreach (var coll in collsToInteract)
        {
            if (coll.TryGetComponent<ItemObject>(out ItemObject itemObject))
            {
                items.Add(itemObject);
            }
        }
        return items.ToArray();
    }
}
