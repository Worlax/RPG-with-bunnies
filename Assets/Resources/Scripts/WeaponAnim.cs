using UnityEngine;

public class WeaponAnim: Animated
{
	// Properties //
	public Transform firePoint;
	public Bullet bulletPrefab;
	public float bulletSpeed = 20f;

	int firesLeft = 0;

	Weapon weapon;
	Vector3 hitLocation;

	// Functions //
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
