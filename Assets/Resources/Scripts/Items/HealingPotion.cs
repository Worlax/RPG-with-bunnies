using UnityEngine;
using UnityEngine.UI;

public class HealingPotion: Usable
{
    // Properties //
    public int HealthForUse = 10;

    // Functions //
    protected override bool UseEffect()
    {
        Stats stats = BattleManager.instance.CurrentUnit.GetComponent<Stats>();

        return stats.Heal(HealthForUse);
    }
}
