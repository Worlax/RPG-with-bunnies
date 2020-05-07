using UnityEngine;
using UnityEngine.UI;

public class StackableItem: Item2D
{
    // Properties //
    Text inStackText;

    public static int maxInStack = 5;
    public int inStack = 1;

    // Functions //
    protected override void Start()
    {
        base.Start();

        inStackText = GetComponentInChildren<Text>();
        inStackText.text = inStack.ToString();
    }

    public void Add(int items)
    {
        inStack += items;

        if (inStack > maxInStack)
        {
            inStack = maxInStack;
        }

        UpdateDisplay();
    }

    public void Subtract(int items)
    {
        inStack -= items;

        if (inStack < 0)
        {
            inStack = 0;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        inStackText.text = inStack.ToString();
    }
}
