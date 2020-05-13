using UnityEngine;

public class WeaponAnim: Animated
{
	// Properties //
	protected Weapon weapon;
	protected Vector3 hitLocation;

	// Functions //
	public virtual void Aim(Transform target)
	{
		animator.SetBool("aim", true);
	}

	public virtual void StopAim()
	{
		animator.SetBool("aim", false);
	}

	public virtual void Fire(Vector3 _hitLocation, Weapon _weapon)
	{
		weapon = _weapon;
		hitLocation = _hitLocation;

		animator.SetBool("fire", true);
	}
}
