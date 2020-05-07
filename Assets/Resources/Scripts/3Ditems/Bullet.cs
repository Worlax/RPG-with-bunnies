using UnityEngine;

public class Bullet: MonoBehaviour
{
    // Properties //
    public GameObject effectPrefab;

    Vector3 contactPoint;
    Stats stats;
    int damage;
    Transform source;

    // Functions //
    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy")
        {
            ParticleSystem particleSystem = Instantiate(effectPrefab).GetComponent<ParticleSystem>();
            particleSystem.transform.position = contactPoint;
            particleSystem.Emit(1);
            
            Destroy(particleSystem.gameObject, particleSystem.main.duration);

            stats.DealDamage(damage, source, true);

            Destroy(gameObject);
        }
    }

    public void Fire(Vector3 _contactPoint, Stats _stats, int _damage, Transform _source)
    {
        contactPoint = _contactPoint;
        stats = _stats;
        damage = _damage;
        source = _source;
    }
}
