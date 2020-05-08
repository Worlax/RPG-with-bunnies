using UnityEngine;

public class Dollar: Item
{
    // Properties //


    // Functions //
    protected override void Start()
    {
        base.Start();

        type = Type.Stackable;
    }
}
