using UnityEngine;
using System;

public class EquipmentSlot: InventorySlot
{
	// Properties //
	UnitController owner;
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

    public override bool ConnectOrSwapItem(Item newItem, bool ignoreMoveTurn = false)
    {
        Equippable equipmentItem = newItem as Equippable;
        if (equipmentItem == null || equipmentItem.SlotType != SlotType)
            return false;

        return base.ConnectOrSwapItem(newItem, ignoreMoveTurn);
    }

    protected override void ConnectItem(Item newItem)
    {
		base.ConnectItem(newItem);

		Equippable item = newItem as Equippable;
		item.EquipItem(owner);
		equipment.AddStats(item);

		if (owner as PlayerController != null && newItem as Weapon != null)
		{
			PlayerEquipedWeapon?.Invoke(newItem as Weapon);
		}
	}

    public override void DisconnectItem()
    {
		Equippable item = itemInSlot as Equippable;
		item.UnequipItem();
		equipment.SubtractStats(item);

		if (owner as PlayerController != null && itemInSlot as Weapon != null)
		{
			PlayerEquipedWeapon?.Invoke(itemInSlot as Weapon);
		}

		base.DisconnectItem();
    }
}
