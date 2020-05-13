using UnityEngine;
using System;

public class Equippable: Item
{
    // Properties //
	public enum EquipType
    {
        None,
        Helmet,
        Body,
        Weapon
    }

    public EquipType equipType;

	// Functions //
	public virtual void EquipItem(UnitController ownerOfThisItem)
    {
		if (ItemVisual != null)
            return;

        ItemVisual = Instantiate(itemIn3DPrefab);
        ItemVisual.transform.SetParent(ownerOfThisItem.transform, false);
	}

    public virtual void UnequipItem()
    {
		if (ItemVisual == null)
            return;

        Destroy(ItemVisual);
		ItemVisual = null;
	}
}
