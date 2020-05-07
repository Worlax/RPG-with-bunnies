using UnityEngine;

public class MyAnimation: MonoBehaviour
{
    // Properties //
    Animator animator;
	
    // Functions //
	void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Foo()
    {
        animator.Play("Idle");
    }
}
