using UnityEngine;

public class MainCamera: MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] bool mouseControlActive = true;
	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float scrollSpeed = 10f;
	[SerializeField] float rotationSpeed = 10f;
	[SerializeField] float boardThickness = 20f;
	[SerializeField] Transform root;

#pragma warning restore 0649

	// Functions //
	void Update()
	{
		// movement
		Vector3 newPosition = transform.localPosition;

		if (Input.GetKey(KeyCode.W) || (mouseControlActive && Input.mousePosition.y > Screen.height - boardThickness))
		{
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.S) || (mouseControlActive && Input.mousePosition.y < 0 + boardThickness))
		{
			transform.position -= transform.forward * moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.A) || (mouseControlActive && Input.mousePosition.x < 0 + boardThickness))
		{
			transform.position -= transform.right * moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D) || (mouseControlActive && Input.mousePosition.x > Screen.width - boardThickness))
		{
			transform.position += transform.right * moveSpeed * Time.deltaTime;
		}

		// scroll
		float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime * -40f;
		transform.position += new Vector3(0, scroll, 0);

		// rotation
		float rotation = 0;

		if (Input.GetKey(KeyCode.Q))
		{
			rotation -= rotationSpeed * Time.deltaTime * 7;
		}
		if (Input.GetKey(KeyCode.E))
		{
			rotation += rotationSpeed * Time.deltaTime * 7;
		}

		transform.Rotate(0, rotation, 0);
	}
}
