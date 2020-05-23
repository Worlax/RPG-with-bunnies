using UnityEngine;

public abstract class Equippable: Item
{
	// Properties //
	[SerializeField] EquipmentSlot.Type _slotType;

	public EquipmentSlot.Type SlotType { get => _slotType; set => _slotType = value; }

	// Functions //
	public virtual void EquipItem(Unit ownerOfThisItem)
    {
		if (ItemVisual != null)
            return;

        ItemVisual = Instantiate(ItemIn3DPrefab);
        ItemVisual.transform.SetParent(ownerOfThisItem.Visual, false);
	}

    public virtual void UnequipItem()
    {
		if (ItemVisual == null)
            return;

        Destroy(ItemVisual);
		ItemVisual = null;
	}
}
