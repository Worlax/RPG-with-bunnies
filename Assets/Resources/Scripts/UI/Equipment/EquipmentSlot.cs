using UnityEngine;
using System;

public class EquipmentSlot: InventorySlot
{
	// Properties //
	Unit owner;
	Equipment equipment;

	// Events //
	public static Action<Weapon> PlayerEquipedWeapon;
	public static Action<Weapon> PlayerUnequipedWeapon;

	public enum Type
	{
		None,
		Helmet,
		Body,
		Weapon
	}

	[SerializeField]
	Type _slotType;
	public Type SlotType { get => _slotType; set => _slotType = value; }

	// Functions //
	void Start()
	{
		equipment = GetComponentInParent<Equipment>();
		owner = equipment.Owner;
	}

    public override bool ConnectOrSwapItem(Item draggedItem)
    {
        Equippable equipmentItem = draggedItem as Equippable;
        if (equipmentItem == null || equipmentItem.SlotType != SlotType)
		{
			return false;
		}

        return base.ConnectOrSwapItem(draggedItem);
    }

    protected override void ConnectItem(Item newItem)
    {
		base.ConnectItem(newItem);

		Equippable item = newItem as Equippable;
		item.EquipItem(owner);
		equipment.AddStats(item);

		if (owner as Player != null && newItem as Weapon != null)
		{
			PlayerEquipedWeapon?.Invoke(newItem as Weapon);
		}
	}

    public override void DisconnectItem()
    {
		if (itemInSlot == null)
			return;

		Equippable item = itemInSlot as Equippable;
		item.UnequipItem();
		equipment.SubtractStats(item);

		if (owner as Player != null && item as Weapon != null)
		{
			PlayerUnequipedWeapon?.Invoke(itemInSlot as Weapon);
		}

		base.DisconnectItem();
    }
}
