using UnityEngine;

public class AnimationScript : MonoBehaviour
{
	// Properties //
	Animator animator;
	public Transform firePoint;
	public Bullet bulletPrefab;
	public float bulletSpeed = 20f;

	public bool AnimationInProcess { get; private set; }
	const float secondsNeededToEndTheAnimation = 0.5f;
	float timeWhenLastAnimationStopped = 0;
	public bool AimAnimationInProcess { get; private set; }
	string propertyInQueue;

	int firesLeft = 0;

	Weapon weapon;
	Vector3 hitLocation;

	// Functions //
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	void Update()
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

	public void Aim(Transform target)
	{
		animator.SetBool("aim", true);
	}

	public void StopAim()
	{
		animator.SetBool("aim", false);
	}

	public void Fire(Vector3 _hitLocation, Weapon _weapon, int fireTimes)
	{
		hitLocation = _hitLocation;
		weapon = _weapon;
		firesLeft = fireTimes;

		animator.SetBool("fire", true);
	}

	void WeaponFired()
	{
		weapon.WeaponFired();

		Transform root = transform.parent;
		root.LookAt(hitLocation);

		Bullet bullet = Instantiate(bulletPrefab);
		bullet.transform.position = firePoint.position;
		bullet.transform.LookAt(hitLocation);

		Vector3 direction = (hitLocation - bullet.transform.position).normalized;
		bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

		bullet.Fire(hitLocation, weapon);

		--firesLeft;
		if (firesLeft <= 0)
		{
			animator.SetBool("fire", false);
		}
	}
}
