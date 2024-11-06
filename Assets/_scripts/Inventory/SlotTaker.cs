using Assets._scripts.Menu;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemObject;

public class SlotTaker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerExitHandler
{
    private Vector2 lastMousePosition;
    private Vector2 defPos;
    public InventorySlot mySlot;
    private Transform myParent;
    [SerializeField] private Image slotStateImage;
    [SerializeField] private Sprite[] slotStates;
    private int siblingIndex;
    private bool stopIt;
    void Start()
    {
        siblingIndex = transform.GetSiblingIndex();
        defPos = transform.localPosition;
    }
    public void Set(InventorySlot slot)
    {
        mySlot = slot;
        myParent = transform.parent;
    }
    public void RePos()
    {
        transform.SetParent(myParent);
        transform.SetSiblingIndex(siblingIndex);
        GetComponent<RectTransform>().localPosition = Vector2.zero;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        stopIt = mySlot.itemInfo == null || mySlot.IsSlotBlocked;
        if (stopIt) { return; }
        //StartDrag
        lastMousePosition = eventData.position;
        transform.SetParent(InventoryManager.GetOutTransform());
        GetComponent<Image>().raycastTarget = false;
        GetComponent<Image>().color = Color.gray;
        InventoryManager.SetActiveDropPanel(true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (stopIt) { return; }
        RectTransform rect = GetComponent<RectTransform>();
        rect.position = Vector2.Lerp(rect.position, Input.mousePosition, 0.5f);
        if (mySlot.slotType == ItemType.def)
        {
            if (eventData.pointerCurrentRaycast.gameObject)
            {
                if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out SlotTaker slotTaker) && slotTaker != this)
                {
                    if (slotTaker.mySlot.itemInfo != null)
                    {
                        Item second = slotTaker.mySlot.itemInfo.item;
                        Item myItem = mySlot.itemInfo.item;
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
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (stopIt) { return; }
        //EndDrag
        if (eventData.pointerCurrentRaycast.gameObject)
        {
            //print(eventData.pointerCurrentRaycast.gameObject.name);
            //if (eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>())
            //{
            //    print("Slot");
            //}
            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out SlotTaker slotTaker) && slotTaker != this)
            {
                if (mySlot.slotType == slotTaker.mySlot.slotType)
                {
                    if (slotTaker.mySlot.itemInfo != null)
                    {
                        Item second = slotTaker.mySlot.itemInfo.item;
                        Item myItem = mySlot.itemInfo.item;
                        if (second == myItem && second.stacable)
                        {
                            int diff = second.item_max_amount - slotTaker.mySlot.itemInfo.amount - mySlot.itemInfo.amount;

                            if (diff < 0)
                            {
                                slotTaker.mySlot.SetSlot(mySlot.itemInfo);
                                mySlot.itemInfo.amount = Mathf.Abs(diff);
                                mySlot.Refresh();
                            }
                            else
                            {
                                slotTaker.mySlot.SetSlot(mySlot.itemInfo);
                                mySlot.ClearSlot();
                            }
                        }
                        else if (second == null)
                        {
                            slotTaker.mySlot.SetSlot(mySlot.itemInfo);
                            mySlot.ClearSlot();
                        }
                        else if (second != myItem)
                        {
                            ItemInfo myCloneItemInfo = new ItemInfo(mySlot.itemInfo);
                            ItemInfo secondCloneItemInfo = new ItemInfo(slotTaker.mySlot.itemInfo);
                            mySlot.ClearSlot();
                            slotTaker.mySlot.ClearSlot();
                            mySlot.SetSlot(secondCloneItemInfo);
                            slotTaker.mySlot.SetSlot(myCloneItemInfo);
                        }
                    }
                    else
                    {
                        if (mySlot.slotType == slotTaker.mySlot.slotType)
                        {
                            slotTaker.mySlot.SetSlot(mySlot.itemInfo);
                            mySlot.ClearSlot();
                        }
                    }
                }
                slotTaker.HideSlotState();
            }
        }
        else
        {
            mySlot.DropSlot();
        }
        transform.SetParent(myParent);
        transform.SetSiblingIndex(siblingIndex);
        GetComponent<RectTransform>().localPosition = defPos;
        GetComponent<Image>().raycastTarget = true;
        if(mySlot.itemInfo != null)
        {
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        InventoryManager.SetActiveDropPanel(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (stopIt) { return; }
        transform.SetParent(myParent);
        transform.SetSiblingIndex(siblingIndex);
        GetComponent<RectTransform>().localPosition = defPos;
        GetComponent<Image>().raycastTarget = true;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == "drop panel")
            {
                mySlot.DropSlot();
            }
        }
        HideSlotState();
    }

    public void SetEmptySlotState()
    {
        if(slotStateImage == null) { return; }
        slotStateImage.sprite = slotStates[0];
        slotStateImage.gameObject.SetActive(true);
    }
    public void SetCollectSlotState()
    {
        if (slotStateImage == null) { return; }
        slotStateImage.sprite = slotStates[1];
        slotStateImage.gameObject.SetActive(true);
    }
    public void SetReplaceSlotState()
    {
        if (slotStateImage == null) { return; }
        slotStateImage.sprite = slotStates[2];
        slotStateImage.gameObject.SetActive(true);
    }
    public void HideSlotState()
    {
        if (slotStateImage == null) { return; }
        slotStateImage.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideSlotState();
    }
}
