using UnityEngine;

public class MeleeWeapon: Weapon
{
	// Properties //
	MeleeWeaponAnim meleeWeaponAnim;

	// Functions //
	public override void EquipItem(Unit ownerOfThisItem)
	{
		base.EquipItem(ownerOfThisItem);

		meleeWeaponAnim = ItemVisual.GetComponentInChildren<MeleeWeaponAnim>();
		meleeWeaponAnim.Weapon = this;
	}

	public override void UnequipItem()
	{
		base.UnequipItem();

		meleeWeaponAnim.Weapon = null;
		meleeWeaponAnim = null;
	}

	public override void Fire()
	{
		meleeWeaponAnim.Fire();
	}
}
