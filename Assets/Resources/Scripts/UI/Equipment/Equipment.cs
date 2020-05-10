using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Equipment: Inventory
{
    // Properties //
    Stats currentStatsDisplayed;

    public Text level;
    public Text exp;
    public Text health;
    public Text damage;

    // Functions //
    void OnEnable()
    {
        PlayerController.OnNewUnitTurn += SetsDisplayedStats;

        Equippable.OnItemEquiped += AddStats;
		Equippable.OnItemUnequiped += SubtractStats;

        Stats.StatsUpdated += UpdateStatsDisplay;
    }

    void OnDisable()
    {
        PlayerController.OnNewUnitTurn -= SetsDisplayedStats;

		Equippable.OnItemEquiped -= AddStats;
		Equippable.OnItemUnequiped -= SubtractStats;

        Stats.StatsUpdated -= UpdateStatsDisplay;
    }

    void SetsDisplayedStats(Stats stats)
    {
        currentStatsDisplayed = stats;

        UpdateStatsDisplay();
    }

    void SetsDisplayedStats(UnitController unit)
    {
        currentStatsDisplayed = unit.GetComponent<Stats>();

        UpdateStatsDisplay();
    }

    public void UpdateStatsDisplay()
    {
        level.text = currentStatsDisplayed.level.ToString();
        exp.text = currentStatsDisplayed.exp.ToString() + " / " + currentStatsDisplayed.expForLevelUp.ToString();
        health.text = currentStatsDisplayed.health.ToString() + " / " + currentStatsDisplayed.maxHealth.ToString();
        damage.text = currentStatsDisplayed.minDamage.ToString() + " - " + currentStatsDisplayed.maxDamage.ToString();
    }

    void AddStats(Equippable item)
    {
        Stats unitStats = GameManager.instance.currentUnit.GetComponent<Stats>();

        if (item is Weapon)
        {
			Weapon weapon = item as Weapon;

			unitStats.AddStats(0, weapon.minDamage, weapon.maxDamage);
        }
        else if (item is Armor)
        {
			Armor armor = item as Armor;

            unitStats.AddStats(armor.maxHealth, 0, 0);
        }
        
        UpdateStatsDisplay();
    }

    void SubtractStats(Equippable item)
    {
        Stats unitStats = GameManager.instance.currentUnit.GetComponent<Stats>();

		if (item is Weapon)
		{
			Weapon weapon = item as Weapon;

			unitStats.SubtractStats(0, weapon.minDamage, weapon.maxDamage);
		}
		else if (item is Armor)
		{
			Armor armor = item as Armor;

			unitStats.SubtractStats(armor.maxHealth, 0, 0);
		}

		UpdateStatsDisplay();
    }
}
