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

        level.text = ownerStats.Level.ToString();
        exp.text = ownerStats.Exp.ToString() + " / " + ownerStats.ExpForLevelUp.ToString();
        health.text = ownerStats.CurrentHealth.ToString() + " / " + ownerStats.MaxHealth.ToString();
        damage.text = ownerStats.MinDamage.ToString() + " - " + ownerStats.MaxDamage.ToString();
    }

    public void AddStats(Equippable item)
    {
        if (item is Weapon)
        {
			Weapon weapon = item as Weapon;

			ownerStats.AddStats(0, weapon.MinDamage, weapon.MaxDamage);
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

			ownerStats.SubtractStats(0, weapon.MinDamage, weapon.MaxDamage);
		}
		else if (item is Armor)
		{
			Armor armor = item as Armor;

			ownerStats.SubtractStats(armor.maxHealth, 0, 0);
		}

		UpdateStatsDisplay();
    }
}
