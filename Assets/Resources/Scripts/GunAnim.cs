using UnityEngine;

public class GunAnim: WeaponAnim
{
	// Properties //
	public Gun Weapon { get; set; }

	public Transform firePoint;
	public Bullet bulletPrefab;
	public float bulletSpeed = 20f;

	protected Vector3 hitLocation;

	int firesLeft = 0;

	// Functions //
	public void Fire(Vector3 _hitLocation, int fireTimes)
	{
		hitLocation = _hitLocation;
		firesLeft = fireTimes;

		Transform root = transform.parent;
		root.LookAt(hitLocation);

		animator.SetBool("fire", true);
	}

	void FireBullet()
	{
		Bullet bullet = Instantiate(bulletPrefab);
		bullet.transform.position = firePoint.position;
		bullet.transform.LookAt(hitLocation);

		Vector3 direction = (hitLocation - bullet.transform.position).normalized;
		bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

		bool lastBullet = false;
		
		if (firesLeft == 1)
		{
			lastBullet = true;
		}

		bullet.Fire(hitLocation, Weapon, lastBullet);

		--firesLeft;
		Weapon.BulletFired();

		if (firesLeft <= 0)
		{
			animator.SetBool("fire", false);
		}
	}
}
