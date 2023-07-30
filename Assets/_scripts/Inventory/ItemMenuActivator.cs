using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMenuActivator : MonoBehaviour, IPointerClickHandler
{
    public InventorySlot slot;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.OpenItemMenu(slot, transform.position);
        }
    }
}
