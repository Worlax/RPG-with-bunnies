using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Item2D: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Properties //
    CanvasGroup canvasGroup;

    InventorySlot lastConnectedSlot;
    public GameObject ItemPrefab;

    // Functions //
    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        GetComponentInParent<InventorySlot>().ConnectOrSwapItem(this, true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lastConnectedSlot.DisconnectItem();

        InventorySlot slot = IsMouseOverSlot();

        if (slot != null)
        {
            if (slot.ConnectOrSwapItem(this) == false)
            {
                lastConnectedSlot.ConnectOrSwapItem(this, true);
            }
        }
        else
        {
            lastConnectedSlot.ConnectOrSwapItem(this, true);
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    InventorySlot IsMouseOverSlot()
    {
        GraphicRaycaster graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        EventSystem eventSystem = GetComponentInParent<EventSystem>();
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();

            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }

    public InventorySlot GetLastConnectedSlot()
    {
        return lastConnectedSlot;
    }

    public void SetLastConnectedSlot(InventorySlot slot)
    {
        lastConnectedSlot = slot;
    }
}
