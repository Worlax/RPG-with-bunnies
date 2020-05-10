using UnityEngine;

public class Bullet: MonoBehaviour
{
    // Properties //
    public GameObject effectPrefab;

    Vector3 hitLocation;
	Weapon weapon;

	// Functions //
	void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy")
        {
            ParticleSystem particleSystem = Instantiate(effectPrefab).GetComponent<ParticleSystem>();
            particleSystem.transform.position = hitLocation;
            particleSystem.Emit(1);

			weapon.WeaponHit();

            Destroy(particleSystem.gameObject, particleSystem.main.duration);
            Destroy(gameObject);
        }
    }

    public void Fire(Vector3 _hitLocation, Weapon _weapon)
    {
        hitLocation = _hitLocation;
		weapon = _weapon;
    }
}
