using UnityEngine;

public class RifleAnim: WeaponAnim
{
	// Properties //
	public Transform firePoint;
	public Bullet bulletPrefab;
	public float bulletSpeed = 20f;

	int firesLeft = 0;

	// Functions //
	public void Fire(Vector3 _hitLocation, Weapon _weapon, int fireTimes)
	{
		hitLocation = _hitLocation;
		weapon = _weapon;
		firesLeft = fireTimes;

		Transform root = transform.parent;
		root.LookAt(hitLocation);

		animator.SetBool("fire", true);
	}

	public override void Fire(Vector3 _hitLocation, Weapon _weapon)
	{
		Fire(_hitLocation, _weapon, 1);
	}

	void WeaponFired()
	{
		weapon.WeaponFired();

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
