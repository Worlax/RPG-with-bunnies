using UnityEngine;

public class Bullet: MonoBehaviour
{
    // Properties //
    public GameObject effectPrefab;

	Transform target;
    Vector3 hitLocation;
	Gun gun;
	bool lastBullet;

	float bulletFiredTime;
	float maxBulletTravelTime = 1.5f;

	// Functions //
	void Update()
	{
		if (Time.time > bulletFiredTime + maxBulletTravelTime)
		{
			Collider collider = GetComponent<Collider>();
			if (collider != null)
			{
				collider.enabled = false;
			}

			BulletHit();
		}
	}

	void OnTriggerEnter(Collider other)
    {
        if (other.transform == target.transform)
        {
			BulletHit();
		}
    }

	public void Fire(Transform _target, Vector3 _hitLocation, Gun _gun, bool _lastBullet)
    {
		target = _target;
		hitLocation = _hitLocation;
		gun = _gun;
		lastBullet = _lastBullet;

		bulletFiredTime = Time.time;
	}

	void BulletHit()
	{
		ParticleSystem particleSystem = Instantiate(effectPrefab).GetComponent<ParticleSystem>();
		particleSystem.transform.position = hitLocation;
		particleSystem.Emit(1);

		gun.WeaponHit();

		if (lastBullet == true)
		{
			gun.WeaponFired();
		}

		Destroy(particleSystem.gameObject, particleSystem.main.duration);
		Destroy(gameObject);
	}
}
