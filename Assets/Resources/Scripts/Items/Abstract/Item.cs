using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class Item: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// Properties //
	public string itemName = "Item";

    CanvasGroup canvasGroup;

	[HideInInspector] public InventorySlot lastConnectedSlot;

    public GameObject itemIn3DPrefab;
    public GameObject ItemVisual { get; protected set; }

    // Functions //
    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

		InventorySlot slot = GetComponentInParent<InventorySlot>();
		if (slot == null)
		{
			Destroy(gameObject);
		}
		else if (slot.ConnectOrSwapItem(this) == false)
		{
			Destroy(gameObject);
		}
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

        InventorySlot slot = GetSlotInMousePosition();

        if (slot != null)
        {
            if (slot.ConnectOrSwapItem(this) == false)
            {
                lastConnectedSlot.ConnectOrSwapItem(this);
            }
        }
        else
        {
            lastConnectedSlot.ConnectOrSwapItem(this);
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    InventorySlot GetSlotInMousePosition()
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
}
