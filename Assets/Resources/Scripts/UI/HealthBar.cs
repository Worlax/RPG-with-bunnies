using UnityEngine;

public class HealthBar: MonoBehaviour
{
	// Properties //
	Quaternion fixedRotation; 
	Vector3 fixedPosition;

	public Transform unit;
	
    // Functions //
	void Start()
	{
		fixedRotation = transform.rotation;
		fixedPosition = transform.position - unit.position;
	}

	void Update()
	{
		transform.rotation = fixedRotation;
		transform.position = unit.position + fixedPosition;
	}
}
