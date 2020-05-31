using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Equipment: Window
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

#pragma warning restore 0649

	Stats ownerStats;

	// Functions //
	protected override void Start()
	{
		base.Start();

		if (Owner is Player && SaveSystem.HaveSavedFile())
		{
			SaveSystem.LoadEquipment();
		}
		else
		{
			InstantiateWithStartPrefabs();
		}
		
		ownerStats = Owner.GetComponent<Stats>();
	}

	protected override void Update()
	{
		base.Update();

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
		if (ownerStats.GetComponent<Enemy>() != null)
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

	public void InstantiatePrefabs(Data data)
	{
		DeleteAllItems();

		InstantiateFromID(data.EquipedHelmet);
		InstantiateFromID(data.EquipedBody);
		InstantiateFromID(data.EquipedWeapon);
	}

	void InstantiateFromID(string id)
	{
		if (id == null)
			return;

		string[] idSplit = id.Split('_');

		Item itemPrefab = DataManager.instance.FindItem(idSplit[0]);

		Item item = Instantiate(itemPrefab);

		foreach (Transform trans in slotsRoot)
		{
			EquipmentSlot slot = trans.GetComponent<EquipmentSlot>();

			if (slot && slot.SlotType == (item as Equippable).SlotType)
			{
				item.transform.SetParent(slot.transform, false);
			}
		}

		if (idSplit.Length >= 3)
		{
			switch (idSplit[1])
			{
				case "weapon":
					(item as Gun).CurrentAmmo = int.Parse(idSplit[2]);
					(item as Gun).AddAmmo(0);
					break;
			}
		}
	}

	public void InstantiatePrefabs(Equippable _helmet, Equippable _body, Weapon _weapon)
	{
		DeleteAllItems();

		if (_helmet != null)
		{
			Equippable helmet = Instantiate(_helmet);
			helmet.transform.SetParent(slotsRoot.GetChild(0), false);
		}
		if (_body != null)
		{
			Equippable body = Instantiate(_body);
			body.transform.SetParent(slotsRoot.GetChild(1), false);
		}
		if (_weapon != null)
		{
			Weapon weapon = Instantiate(_weapon);
			weapon.transform.SetParent(slotsRoot.GetChild(2), false);
		}
	}

	void InstantiateWithStartPrefabs()
	{
		InstantiatePrefabs(helmet, body, weapon);
	}

	public EquipmentSlot[] GetAllSlots()
	{
		List<EquipmentSlot> slots = new List<EquipmentSlot>();

		foreach (Transform transform in slotsRoot)
		{
			EquipmentSlot slot = transform.GetComponent<EquipmentSlot>();

			if (slot != null)
				slots.Add(slot);
		}

		return slots.ToArray();
	}

	void DeleteAllItems()
	{
		foreach (EquipmentSlot slot in GetAllSlots())
		{
			if (!slot.IsEmpty())
			{
				Equippable item = slot.GetItem() as Equippable;
				slot.DisconnectItem();

				Destroy(item.gameObject);
			}	
		}
	}
}


