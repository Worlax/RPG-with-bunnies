using UnityEngine.UI;

public abstract class Stackable: Item
{
    // Properties //
    Text inStackText;

    public int maxInStack;
    public int inStack = 1;

    // Functions //
    protected override void Start()
    {
        base.Start();

        inStackText = GetComponentInChildren<Text>();

		UpdateVisual();
	}

    public void AddToStack(int amount)
    {
        inStack += amount;
        if (inStack > maxInStack)
        {
            inStack = maxInStack;
        }

		UpdateVisual();
    }

    public void TakeFromStack(int amount, out int wasTaken)
    {
		if (inStack >= amount)
		{
			wasTaken = amount;
			inStack -= amount;
		}
		else
		{
			wasTaken = inStack;
			inStack = 0;
		}

		UpdateVisual();

		if (inStack <= 0)
		{
			Destroy(gameObject);
		}
	}

	void UpdateVisual()
	{
		inStackText.text = inStack.ToString();
	}
}
