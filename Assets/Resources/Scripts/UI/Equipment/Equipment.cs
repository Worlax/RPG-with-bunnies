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

        EquipmentSlot.OnItemEquiped += AddStats;
        EquipmentSlot.OnItemUnequiped += RemoveStats;

        Stats.StatsUpdated += UpdateStatsDisplay;
    }

    void OnDisable()
    {
        PlayerController.OnNewUnitTurn -= SetsDisplayedStats;

        EquipmentSlot.OnItemEquiped -= AddStats;
        EquipmentSlot.OnItemUnequiped -= RemoveStats;

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

    void AddStats(Item2D item, Item3D itemIn3d)
    {
        Stats unitStats = GameManager.instance.currentUnit.GetComponent<Stats>();

        Weapon weapon = itemIn3d as Weapon;

        if (weapon)
        {
            unitStats.AddStats(0, weapon.minDamage, weapon.maxDamage);
        }
        else
        {
            unitStats.AddStats(itemIn3d.maxHealth, 0, 0);
        }
        
        UpdateStatsDisplay();
    }

    void RemoveStats(Item2D item, Item3D itemIn3d)
    {
        Stats unitStats = GameManager.instance.currentUnit.GetComponent<Stats>();

        Weapon weapon = itemIn3d as Weapon;

        if (weapon)
        {
            unitStats.SubtractStats(0, weapon.minDamage, weapon.maxDamage);
        }
        else
        {
            unitStats.SubtractStats(itemIn3d.maxHealth, 0, 0);
        }

        UpdateStatsDisplay();
    }
}
