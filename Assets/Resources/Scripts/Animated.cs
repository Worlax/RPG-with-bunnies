using UnityEngine;

public class Animated: MonoBehaviour
{
	// Properties //
	protected Animator animator;

	public bool AnimationInProcess { get; private set; }

	const float secondsNeededToEndTheAnimation = 0.5f;
	float timeWhenLastAnimationStopped = 0;

	// Functions //
	protected virtual void Start()
	{
		animator = GetComponent<Animator>();
	}

	protected virtual void Update()
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
