using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class Item: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// Properties //
	public string itemName = "Item";

#pragma warning disable 0649

	[SerializeField] int _price = 100; 
	[SerializeField] GameObject _itemIn3DPrefab;

#pragma warning restore 0649

	//
	public int Price { get => _price; }
	public GameObject ItemIn3DPrefab { get => _itemIn3DPrefab; }
    public GameObject ItemVisual { get; protected set; }

	[HideInInspector] public InventorySlot lastConnectedSlot;
    CanvasGroup canvasGroup;

    // Functions //
    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

		InventorySlot slot = GetComponentInParent<InventorySlot>();
		if (slot == null)
		{
			Destroy(gameObject);
		}
		else
		{
			slot.ConnectOrSwapItem(this, true);
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
