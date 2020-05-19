using UnityEngine;
using UnityEngine.UI;

public class Equipment : Window
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] RectTransform slotsRoot;

	[Header("Visual Display")]
	[SerializeField] Text level;
	[SerializeField] Text exp;
	[SerializeField] Text health;
	[SerializeField] Text damage;

	[Header("Start items")]
	[SerializeField] Equippable helmet;
	[SerializeField] Equippable body;
	[SerializeField] Weapon weapon;

	Stats ownerStats;

#pragma warning restore 0649
	// Functions //
	protected override void Start()
	{
		base.Start();

		InstantiatePrefabs();

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

	void InstantiatePrefabs()
	{
		if (helmet != null)
		{
			Equippable _helmet = Instantiate(helmet);
			_helmet.transform.SetParent(slotsRoot.GetChild(0), false);
		}
		if (body != null)
		{
			Equippable _body = Instantiate(body);
			_body.transform.SetParent(slotsRoot.GetChild(1), false);
		}
		if (weapon != null)
		{
			Weapon _weapon = Instantiate(weapon);
			_weapon.transform.SetParent(slotsRoot.GetChild(2), false);
		}
	}
}
