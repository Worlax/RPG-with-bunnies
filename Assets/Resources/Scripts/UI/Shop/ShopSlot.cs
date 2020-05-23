using UnityEngine;

public class ShopSlot: InventorySlot
{
	// Properties //
	public enum Type
	{
		Shop,
		Buy,
		Sell
	}

	[SerializeField] Type _slotType;
	public Type SlotType { get => _slotType; private set => _slotType = value; }

	// Functions //
	public override bool ConnectOrSwapItem(Item draggedItem)
	{
		ShopSlot shopSlot = draggedItem.lastConnectedSlot as ShopSlot;

		if (SlotType == Type.Shop || SlotType == Type.Buy)
		{
			if (shopSlot == null || shopSlot.SlotType == Type.Sell)
				return false;
		}
		else if (SlotType == Type.Sell)
		{
			if (shopSlot != null && shopSlot.SlotType != Type.Sell)
				return false;
		}

		return base.ConnectOrSwapItem(draggedItem);
	}
}
