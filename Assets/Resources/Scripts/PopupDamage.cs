using UnityEngine;

public class PopupDamage: MonoBehaviour
{
    // Properties //
    public float lifetime = 3f;
	
    // Functions //
	void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
