using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Equipment: Inventory
{
    // Properties //
    Stats playerStats;

    public Text level;
    public Text exp;
    public Text health;
    public Text damage;

    // Functions //
    void OnEnable()
    {
        PlayerController.OnNewUnitTurn += UpdateStatsDisplay;

        EquipmentSlot.OnItemEquiped += AddStats;
        EquipmentSlot.OnItemUnequiped += RemoveStats;
    }

    void OnDisable()
    {
        PlayerController.OnNewUnitTurn -= UpdateStatsDisplay;

        EquipmentSlot.OnItemEquiped -= AddStats;
        EquipmentSlot.OnItemUnequiped -= RemoveStats;
    }
    
    public void UpdateStatsDisplay(UnitController unit)
    {
        PlayerController player = unit.GetComponent<PlayerController>();

        if (player == null)
            return;

        playerStats = player.GetComponent<Stats>();

        level.text = playerStats.level.ToString();
        exp.text = playerStats.exp.ToString() + " / " + playerStats.expForLevelUp.ToString();
        health.text = playerStats.health.ToString() + " / " + playerStats.maxHealth.ToString();
        damage.text = playerStats.minDamage.ToString() + " - " + playerStats.maxDamage.ToString();
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
        
        UpdateStatsDisplay(GameManager.instance.currentUnit);
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

        UpdateStatsDisplay(GameManager.instance.currentUnit);
    }
}
