using UnityEngine;

public abstract class WeaponAnim: Animated
{
	// Properties //

	
	// Functions //
	public virtual void Aim(Transform target)
	{
		animator.SetBool("aim", true);
	}

	public virtual void StopAim()
	{
		animator.SetBool("aim", false);
	}
}
