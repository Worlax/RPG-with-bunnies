using UnityEngine;
using System.Collections.Generic;

public class DataManager: MonoBehaviour
{
	// Singleton //
	public static DataManager instance = null;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		FillPrefabList();
	}

	// Properties //
	List<Item> itemsPrefabs = new List<Item>();
	
    // Functions //
	public Data GetData()
	{
		Player player = GameManager.instance.CurrenPlayer;
		Data data = new Data();

		// money
		data.Money = player.Money;

		// inventory items
		Item[] inventoryItems = player.Inventory.GetAllItems();
		string[] inventoryItemsIDs = GetIDs(inventoryItems);

		data.InventoryItems = inventoryItemsIDs;

		// equipment items
		EquipmentSlot[] slots = player.Equipment.GetAllSlots();

		foreach (EquipmentSlot slot in slots)
		{
			if (!slot.IsEmpty())
			{
				switch (slot.SlotType)
				{
					case EquipmentSlot.Type.Helmet:
						data.EquipedHelmet = slot.GetItem().GetID();
						break;

					case EquipmentSlot.Type.Body:
						data.EquipedBody = slot.GetItem().GetID();
						break;

					case EquipmentSlot.Type.Weapon:
						data.EquipedWeapon = slot.GetItem().GetID();
						break;
				}
			}
		}

		// stats
		data.Level = player.Stats.Level;
		data.CurrentHealth = player.Stats.CurrentHealth;
		data.Exp = player.Stats.Exp;
		data.ExpForLevelUp = player.Stats.ExpForLevelUp;

		return data;
	}

	public Item[] GetInventoryItems(Data data)
	{
		return GetItems(data.InventoryItems);
	}

	public void GetEquipedItems(Data data, out Equippable helmet, out Equippable body, out Weapon weapon)
	{
		helmet = GetItem(data.EquipedHelmet) as Equippable;
		body = GetItem(data.EquipedBody) as Equippable;
		weapon = GetItem(data.EquipedWeapon) as Weapon;
	}

	string[] GetIDs(Item[] items)
	{
		string[] itemsID = new string[items.Length];

		for (int i = 0; i < items.Length; ++i)
		{
			itemsID[i] = items[i].GetID();
		}

		return itemsID;
	}

	Item GetItem(string itemID)
	{
		if (itemID == null)
			return null;

		string[] idSplit = itemID.Split('_');

		Item item = FindItem(idSplit[0]);

		if (idSplit.Length >= 3)
		{
			switch (idSplit[1])
			{
				case "usable":
					(item as Usable).usesLeft = int.Parse(idSplit[2]);
					break;

				case "stackable":
					(item as Stackable).inStack = int.Parse(idSplit[2]);
					break;

				case "weapon":
					(item as Gun).CurrentAmmo = int.Parse(idSplit[2]);
					break;
			}
		}

		return item;
	}

	Item[] GetItems(string[] itemsID)
	{
		List<Item> items = new List<Item>();

		foreach (string itemID in itemsID)
		{
			items.Add(GetItem(itemID));
		}
		
		return items.ToArray();
	}

	public Item FindItem(string itemName)
	{
		foreach (Item item in itemsPrefabs)
		{
			if (item.ItemName == itemName)
			{
				return item;
			}
		}

		return null;
	}

	void FillPrefabList()
	{
		UnityEngine.Object[] objPrefabs = Resources.LoadAll("Prefabs", typeof(Item));

		foreach (UnityEngine.Object obj in objPrefabs)
		{
			if (obj is Item) { itemsPrefabs.Add(obj as Item); }
		}
	}
}
