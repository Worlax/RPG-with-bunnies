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

	protected override void IsAnimationInProcessCheck()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.IsInTransition(0) == false)
		{
			if (timeWhenLastAnimationStopped == 0)
			{
				timeWhenLastAnimationStopped = Time.time;
			}
			if ((Time.time - timeWhenLastAnimationStopped) > secondsNeededToEndTheAnimation)
			{
				timeWhenLastAnimationStopped = 0;
				AnimationInProcess = false;
			}
		}
		else
		{
			AnimationInProcess = true;
		}
	}
}
