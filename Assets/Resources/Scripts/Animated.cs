using UnityEngine;

public class Animated: MonoBehaviour
{
	// Properties //
	protected Animator animator;

	public bool AnimationInProcess { get; protected set; }

	protected const float secondsNeededToEndTheAnimation = 0.5f;
	protected float timeWhenLastAnimationStopped = 0;

	// Functions //
	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	protected virtual void Update()
	{
		IsAnimationInProcessCheck();
	}

	protected virtual void IsAnimationInProcessCheck()
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

	public void Disable()
	{
		animator.enabled = false;
		this.enabled = false;
	}
}
