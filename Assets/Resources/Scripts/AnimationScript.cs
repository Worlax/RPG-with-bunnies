using UnityEngine;

public class AnimationScript : MonoBehaviour
{
	// Properties //
	Animator animator;
	public Transform firePoint;
	public Bullet bulletPrefab;
	public float bulletSpeed = 20f;

	bool aimAnimationInProcess;
	public bool AimAnimationInProcess { get; private set; }

	int firesLeft = 0;

	Weapon weapon;
	Vector3 hitLocation;

	// Functions //
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	void AnimationEnded()
	{
		aimAnimationInProcess = false;
	}

	public void Aim(Transform target)
	{
		animator.SetBool("aim", true);
		aimAnimationInProcess = true;
	}

	public void StopAim()
	{
		animator.SetBool("aim", false);
		aimAnimationInProcess = true;
	}

	public void Fire(Vector3 _hitLocation, Weapon _weapon, int fireTimes)
	{
		hitLocation = _hitLocation;
		weapon = _weapon;
		firesLeft = fireTimes;

		animator.Play("Fire");
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
	}

	void WeaponFireAnimationEnd()
	{
		--firesLeft;

		if (firesLeft > 0)
		{
			animator.Play("Fire");
		}
		else
		{
			AnimationEnded();
		}
	}
}
