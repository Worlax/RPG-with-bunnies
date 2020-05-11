using UnityEngine;
using UnityEngine.UI;

public class Usable: Item
{
    // Properties //
    public int maxUses = 3;
    public int usesLeft;

	Slider usesSlider; 
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

    protected virtual bool UseEffect()
    {
        return true;
    }
}
