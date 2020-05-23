using UnityEngine;
using UnityEngine.UI;

public class HealingPotion: Usable
{
    // Properties //
    public int HealthForUse = 10;

    // Functions //
    protected override bool UseEffect(Unit user)
    {
        Stats stats = user.GetComponent<Stats>();

        return stats.Heal(HealthForUse);
    }
}
