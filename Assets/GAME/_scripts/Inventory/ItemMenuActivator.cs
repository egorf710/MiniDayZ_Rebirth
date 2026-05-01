using System.Collections;
using System.Collections.Generic;
using Assets.GAME._scripts.Fic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMenuActivator : MonoBehaviour, IPointerClickHandler
{
    public InventorySlot slot;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && !slot.SlotIsNull())
        {
            ServiceLocator.Get<S_Inventory>().OpenItemMenu(slot, transform.position);
        }
    }
}
