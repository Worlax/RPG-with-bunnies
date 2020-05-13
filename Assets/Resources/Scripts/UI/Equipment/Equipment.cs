using UnityEngine;
using UnityEngine.UI;

public class Equipment: Inventory
{
    // Properties //
    public Text level;
    public Text exp;
    public Text health;
    public Text damage;

	Stats ownerStats;

	// Functions //
	protected override void Start()
	{
		base.Start();

		ownerStats = Owner.GetComponent<Stats>();
		UpdateStatsDisplay();
	}

	void OnEnable()
    {
        Stats.StatsUpdated += UpdateStatsDisplay;
    }

    void OnDisable()
    {
        Stats.StatsUpdated -= UpdateStatsDisplay;
    }

    public void UpdateStatsDisplay()
    {
		if (ownerStats.GetComponent<EnemyController>() != null)
			return;

        level.text = ownerStats.level.ToString();
        exp.text = ownerStats.exp.ToString() + " / " + ownerStats.expForLevelUp.ToString();
        health.text = ownerStats.currentHealth.ToString() + " / " + ownerStats.maxHealth.ToString();
        damage.text = ownerStats.minDamage.ToString() + " - " + ownerStats.maxDamage.ToString();
    }

    public void AddStats(Equippable item)
    {
        if (item is Weapon)
        {
			Weapon weapon = item as Weapon;

			ownerStats.AddStats(0, weapon.minDamage, weapon.maxDamage);
        }
        else if (item is Armor)
        {
			Armor armor = item as Armor;

			ownerStats.AddStats(armor.maxHealth, 0, 0);
        }
        
        UpdateStatsDisplay();
    }

	public void SubtractStats(Equippable item)
    {
		if (item is Weapon)
		{
			Weapon weapon = item as Weapon;

			ownerStats.SubtractStats(0, weapon.minDamage, weapon.maxDamage);
		}
		else if (item is Armor)
		{
			Armor armor = item as Armor;

			ownerStats.SubtractStats(armor.maxHealth, 0, 0);
		}

		UpdateStatsDisplay();
    }
}
