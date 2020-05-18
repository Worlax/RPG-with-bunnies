using UnityEngine;

public abstract class Animated: MonoBehaviour
{
	// Properties //
	protected Animator animator;

	// Functions //
	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void Disable()
	{
		animator.enabled = false;
		this.enabled = false;
	}
}
