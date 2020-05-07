using UnityEngine;
using UnityEngine.UI;

public class HealingPotion: ConsumableItem
{
    // Properties //
    public int HealthForUse = 10;

    // Functions //
    protected override bool UseEffect()
    {
        Stats stats = GameManager.instance.currentUnit.GetComponent<Stats>();
        return stats.Heal(HealthForUse);
    }
}
