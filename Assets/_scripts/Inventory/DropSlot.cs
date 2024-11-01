using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemObject;

public class DropSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemInfo itemInfo;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemText;
    private int siblingIndex;
    private Vector2 lastMousePosition;
    private Transform myParent;
    private ItemObject myObject;
    public Vector3 defPos;

    void Start()
    {
        siblingIndex = transform.GetSiblingIndex();
        myParent = itemIcon.transform.parent;
        defPos = itemIcon.transform.localPosition;
    }
    private void OnDestroy()
    {
        Destroy(itemIcon.gameObject);
    }
    public void SetSlot(ItemObject itemObject)
    {
        itemInfo = itemObject.itemInfo;
        myObject = itemObject;
        itemIcon.sprite = itemObject.itemInfo.item.item_sprite;
        itemText.text = itemInfo.item.item_name;
        if(itemInfo.item is weaponItem && itemInfo.ammo > 0)
        {
            itemText.text += "\n" + itemInfo.ammo;
        }
        else if(itemInfo.item.stacable && itemInfo.amount > 1)
        {
            itemText.text += "\n" + itemInfo.amount;
        }
        else if(itemInfo.item is armorItem || itemInfo.item.item_type == ItemType.melee)
        {
            itemText.text += "\n(" + itemInfo.durability + ")%";
        }
    }
    private void Refresh()
    {
        itemText.text = itemInfo.item.item_name;
        if (itemInfo.item is weaponItem)
        {
            itemText.text += "\n" + itemInfo.ammo;
        }
        else if (itemInfo.item.stacable && itemInfo.amount > 1)
        {
            itemText.text += "\n" + itemInfo.amount;
        }
        else if (itemInfo.item is armorItem || itemInfo.item.item_type == ItemType.melee)
        {
            itemText.text += "\n(" + itemInfo.durability + ")%";
        }
    }
    public void RePos()
    {
        itemIcon.transform.SetParent(myParent);
        itemIcon.transform.SetSiblingIndex(siblingIndex);
        itemIcon.transform.localPosition = Vector2.zero;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //StartDrag
        lastMousePosition = eventData.position;
        itemIcon.transform.SetParent(InventoryManager.GetOutTransform());
        itemIcon.raycastTarget = false;
        itemIcon.color = Color.gray;
    }
    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rect = itemIcon.GetComponent<RectTransform>();
        rect.position = Vector2.Lerp(rect.position, Input.mousePosition, 0.5f);
        if (eventData.pointerCurrentRaycast.gameObject)
        {
            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out SlotTaker slotTaker))
            {
                if (slotTaker.mySlot.slotType == ItemType.def)
                {
                    if (slotTaker.mySlot.itemInfo != null)
                    {
                        Item second = slotTaker.mySlot.itemInfo.item;
                        Item myItem = itemInfo.item;
                        if (second == myItem && second.stacable)
                        {
                            slotTaker.SetCollectSlotState();
                        }
                        else if (second == null)
                        {
                            slotTaker.SetEmptySlotState();
                        }
                        else if (second != myItem)
                        {
                            slotTaker.SetReplaceSlotState();
                        }
                    }
                    else
                    {
                        slotTaker.SetEmptySlotState();
                    }
                }
                else
                {

                }
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //EndDrag
        if (eventData.pointerCurrentRaycast.gameObject)
        {
            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out SlotTaker slotTaker))
            {
                if (slotTaker.mySlot.slotType == ItemType.def)
                {
                    if (slotTaker.mySlot.itemInfo != null)
                    {
                        Item second = slotTaker.mySlot.itemInfo.item;
                        Item myItem = itemInfo.item;
                        if (second == myItem && second.stacable)
                        {
                            int diff = second.item_max_amount - slotTaker.mySlot.itemInfo.amount - itemInfo.amount;

                            if (diff < 0)
                            {
                                slotTaker.mySlot.SetSlot(itemInfo);
                                itemInfo.amount = Mathf.Abs(diff);
                                Refresh();
                            }
                            else
                            {
                                slotTaker.mySlot.SetSlot(itemInfo);
                                Destroy(myObject.gameObject);
                                Destroy(gameObject);
                            }
                        }
                        else if (second == null)
                        {
                            slotTaker.mySlot.SetSlot(itemInfo);
                            Destroy(myObject.gameObject);
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        if (itemInfo.item.item_type == ItemType.def || itemInfo.item.item_type == ItemType.food)
                        {
                            slotTaker.mySlot.SetSlot(itemInfo);
                            Destroy(myObject.gameObject);
                            Destroy(gameObject);
                        }
                    }
                    slotTaker.HideSlotState();
                }
            }
            else
            {
                if (InventoryManager.AddItem(itemInfo))
                {
                    ServerManager.DestroyItemObjectAtID(myObject.gameObject.GetComponent<IdentityObject>().ID);
                    //Destroy(myObject.gameObject);
                    Destroy(gameObject);
                }
            }
        }
        itemIcon.transform.SetParent(myParent);
        itemIcon.transform.SetSiblingIndex(siblingIndex);
        itemIcon.transform.localPosition = defPos;
        itemIcon.raycastTarget = true;
        if (itemInfo != null)
        {
            itemIcon.color = Color.white;
        }
        else
        {
            itemIcon.color = new Color(1, 1, 1, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount > 1)
        {
            if (InventoryManager.AddItem(itemInfo))
            {
                ServerManager.DestroyItemObjectAtID(myObject.gameObject.GetComponent<IdentityObject>().ID);

                //Destroy(myObject.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
