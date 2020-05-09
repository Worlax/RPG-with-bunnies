using UnityEngine;

public class Usable: Item
{
    // Properties //
    public int maxUses;
    public int usesLeft;

    // Functions //
    protected override void Start()
    {
        base.Start();

        usesLeft = maxUses;
    }

    public void Use()
    {
        if (UseEffect() == true)
        {
            --usesLeft;

            if (usesLeft == 0)
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
