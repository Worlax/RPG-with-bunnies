using UnityEngine;
using UnityEngine.UI;

public class Inventory: MonoBehaviour
{
    // Properties //
    public KeyCode toggleKey;
    public Button closeButton;

	UnitController owner;
	public UnitController Owner { get => owner; set { if (owner == null) { owner = value; } } }

    // Functions //
    protected virtual void Start()
    {
        closeButton.onClick.AddListener(Close);
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
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }

	void Close()
	{
		transform.localScale = Vector3.zero;
	}
}
