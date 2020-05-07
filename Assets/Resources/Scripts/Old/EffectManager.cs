using UnityEngine;

public class EffectManager: MonoBehaviour
{
    // Singleton //
    public static EffectManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Properties //
    public ParticleSystem explosion;

    // Functions //

}
