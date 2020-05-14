using UnityEngine;

public class Bullet: MonoBehaviour
{
    // Properties //
    public GameObject effectPrefab;

    Vector3 hitLocation;
	Gun gun;
	bool lastBullet;

	// Functions //
	void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy")
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

    public void Fire(Vector3 _hitLocation, Gun _gun, bool _lastBullet)
    {
        hitLocation = _hitLocation;
		gun = _gun;
		lastBullet = _lastBullet;
    }
}
