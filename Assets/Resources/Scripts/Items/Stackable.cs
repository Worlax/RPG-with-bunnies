using UnityEngine;
using UnityEngine.UI;

public class Stackable: Item
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
        inStackText.text = inStack.ToString();
    }

    public void AddToStack(int amount)
    {
        inStack += amount;
        if (inStack > maxInStack)
        {
            inStack = maxInStack;
        }

        inStackText.text = inStack.ToString();
    }

    public void SubtractFromStack(int amount)
    {
        inStack -= amount;
        if (inStack <= 0)
        {
            Destroy(gameObject);
        }

        inStackText.text = inStack.ToString();
    }
}
