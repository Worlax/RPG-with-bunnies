using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Usable: Item, IPointerClickHandler
{
    // Properties //
    public int maxUses = 3;
    public int usesLeft;

	Slider usesSlider;

	float lastTimeClicked = 0;
	const float doubleClickMaxSpread = 0.25f;
	// Functions //
	protected override void Start()
    {
        base.Start();

		if (usesLeft == 0)
		{
			usesLeft = maxUses;
		}
        
		usesSlider = GetComponentInChildren<Slider>();
		UpdateVisual();
	}

    public void Use(Unit user)
    {
        if (UseEffect(user) == true)
        {
            --usesLeft;
			UpdateVisual();

			if (usesLeft <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

	public void UpdateVisual()
	{
		usesSlider.value = (float)usesLeft / (float)maxUses;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (lastTimeClicked == 0)
		{
			lastTimeClicked = Time.time;
			return;
		}
		else if (Time.time - lastTimeClicked < doubleClickMaxSpread)
		{
			Use(GameManager.instance.CurrenPlayer);
		}

		lastTimeClicked = Time.time;
	}

	protected abstract bool UseEffect(Unit user);
}
