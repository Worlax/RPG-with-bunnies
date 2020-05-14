using UnityEngine;

public class MeleeWeaponAnim: WeaponAnim
{
	// Properties //
	public MeleeWeapon Weapon { get; set; }

	// Functions //
	public void Fire()
	{
		animator.SetBool("fire", true);
	}

	public void WeaponHit()
	{
		Weapon.WeaponHit();
	}

	void WeaponFired()
	{
		Weapon.WeaponFired();
	}
}
