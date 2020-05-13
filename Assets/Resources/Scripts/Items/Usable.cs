using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Usable: Item, IPointerClickHandler
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

        usesLeft = maxUses;
		usesSlider = GetComponentInChildren<Slider>();
		usesSlider.value = 1;
	}

    public void Use()
    {
        if (UseEffect() == true)
        {
            --usesLeft;
			usesSlider.value = (float)usesLeft / (float)maxUses;

			if (usesLeft <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

	public void OnPointerClick(PointerEventData eventData)
	{
		if (lastTimeClicked == 0)
		{
			lastTimeClicked = Time.time;
			return;
		}
		else if (Time.time - lastTimeClicked < doubleClickMaxSpread && GameManager.instance.playerMove == true)
		{
			Use();
		}

		lastTimeClicked = Time.time;
	}

	protected virtual bool UseEffect()
    {
        return true;
    }
}
