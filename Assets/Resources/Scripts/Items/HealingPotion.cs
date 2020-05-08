using UnityEngine;
using UnityEngine.UI;

public class HealingPotion: Item
{
    // Properties //
    public int HealthForUse = 10;

    // Functions //
    protected override void Start()
    {
        base.Start();

        type = Type.Usable;
    }

    protected override bool UseEffect()
    {
        Stats stats = GameManager.instance.currentUnit.GetComponent<Stats>();

        return stats.Heal(HealthForUse);
    }
}
