using UnityEngine;
using UnityEngine.UI;

public class Window: MonoBehaviour
{
	// Properties //
	[SerializeField] KeyCode toggleKey = default;
	[SerializeField] Button closeButton = null;

	UnitController owner;
	public UnitController Owner { get => owner; set { if (owner == null) { owner = value; } } }

	public bool Closed { get; private set; }

	// Functions //
	protected virtual void Start()
	{
		closeButton.onClick.AddListener(Close);

		if (transform.localScale == Vector3.zero)
		{
			Closed = true;
		}
		else
		{
			Closed = false;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			ToggleVisibility();
		}
	}

	void ToggleVisibility()
	{
		if (transform.localScale == Vector3.zero)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void Open()
	{
		transform.localScale = Vector3.one;
		Closed = false;
	}

	public void Close()
	{
		transform.localScale = Vector3.zero;
		Closed = true;
	}
}
