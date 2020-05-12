using UnityEngine;
using System;

public class Equippable: Item
{
    // Properties //
	protected WeaponAnim animationScript;

	public enum EquipType
    {
        None,
        Helmet,
        Body,
        Weapon
    }

    public EquipType equipType;

    // Events //
    public static event Action<Equippable> OnItemEquiped;
    public static event Action<Equippable> OnItemUnequiped;

	// Functions //
	public virtual void EquipItem()
    {
		if (itemVisual != null)
            return;

        itemVisual = Instantiate(itemIn3DPrefab);
        itemVisual.transform.SetParent(GameManager.instance.currentUnit.transform, false);
		animationScript = itemVisual.GetComponentInChildren<WeaponAnim>();

        OnItemEquiped(this);
	}

    public virtual void UnequipItem()
    {
		if (itemVisual == null)
            return;

        OnItemUnequiped(this);

        Destroy(itemVisual);
		itemVisual = null;
	}
}
