using UnityEngine;
using UnityEngine.UI;

public class Inventory: MonoBehaviour
{
    // Properties //
    public KeyCode toggleKey;
    public Button closeButton;

    // Functions //
    protected virtual void Start()
    {
        closeButton.onClick.AddListener(ToggleVisibility);
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
}
