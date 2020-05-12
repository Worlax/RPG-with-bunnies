using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlot: InventorySlot
{
    // Properties //
    public Equippable.EquipType type;

    // Functions //
    public override bool ConnectOrSwapItem(Item newItem, bool ignoreMoveTurn = false)
    {
        Equippable equipmentItem = newItem as Equippable;
        if (equipmentItem == null || equipmentItem.equipType != type)
            return false;

        return base.ConnectOrSwapItem(newItem, ignoreMoveTurn);
    }

    protected override void ConnectItem(Item newItem)
    {
		base.ConnectItem(newItem);
		(itemInSlot as Equippable).EquipItem();
    }

    public override void DisconnectItem()
    {
		(itemInSlot as Equippable).UnequipItem();
		base.DisconnectItem();
    }
}
