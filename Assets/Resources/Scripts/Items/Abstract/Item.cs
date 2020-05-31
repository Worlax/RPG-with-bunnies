using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class Item: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// Properties //

#pragma warning disable 0649

	[SerializeField] string _itemName = "Default";
	[SerializeField] int _price = 100; 
	[SerializeField] GameObject _itemIn3DPrefab;

#pragma warning restore 0649

	//
	public string ItemName { get => _itemName; }
	public int Price { get => _price; }
	public GameObject ItemIn3DPrefab { get => _itemIn3DPrefab; }
    public GameObject ItemVisual { get; protected set; }

	[HideInInspector] public InventorySlot lastConnectedSlot;
    CanvasGroup canvasGroup;

	// Functions //
	protected virtual void Start()
    {
		if (_itemName == "Default")
		{
			Debug.LogWarning("Item '" + this + "' have a default name!");
		}

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

	public string GetID()
	{
		string ID = _itemName;

		if (this is Usable)
		{
			ID += "_" + "usable" + "_" + (this as Usable).usesLeft.ToString();
		}
		else if (this is Stackable)
		{
			ID += "_" + "stackable" + "_" + (this as Stackable).inStack.ToString();
		}
		else if (this is Gun)
		{
			ID += "_" + "weapon" + "_" + (this as Gun).CurrentAmmo.ToString();
		}

		return ID;
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
