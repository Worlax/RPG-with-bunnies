using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
	// Properties //
	// folders ("p" for "file Path")
	static readonly string pData = Application.persistentDataPath;
	static readonly string pPlayerData = Application.persistentDataPath + "/player";

	// files
	static readonly string pPlayerItems = pPlayerData + "/items.dat";

	// Functions //
	static SaveSystem()
	{
		Directory.CreateDirectory(pPlayerData);
	}

	public static void Save()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		Data data = DataManager.instance.GetData();

		using (FileStream stream = new FileStream(pPlayerItems, FileMode.Create))
		{
			formatter.Serialize(stream, data);
		}
	}

	public static void Load()
	{
		LoadPlayer();
	}

	public static bool HaveSavedFile()
	{
		return File.Exists(pPlayerItems);
	}

	public static void LoadInventory()
	{
		if (File.Exists(pPlayerItems))
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (FileStream stream = new FileStream(pPlayerItems, FileMode.Open))
			{
				Player player = GameManager.instance.CurrenPlayer;
				Data data = formatter.Deserialize(stream) as Data;

				// money
				player.Money = data.Money;
				player.AddMoney(0);

				// inventory items
				player.Inventory.InstantiatePrefabs(data);
			}
		}
	}

	public static void LoadEquipment()
	{
		if (File.Exists(pPlayerItems))
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (FileStream stream = new FileStream(pPlayerItems, FileMode.Open))
			{
				Player player = GameManager.instance.CurrenPlayer;
				Data data = formatter.Deserialize(stream) as Data;

				// equiped items
				player.Equipment.InstantiatePrefabs(data);

				// stats
				player.Stats.Level = data.Level;
				player.Stats.CurrentHealth = data.CurrentHealth;
				player.Stats.Exp = data.Exp;
				player.Stats.ExpForLevelUp = data.ExpForLevelUp;
			}
		}
	}

	static bool LoadPlayer()
	{
		if (File.Exists(pPlayerItems))
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (FileStream stream = new FileStream(pPlayerItems, FileMode.Open))
			{
				Player player = GameManager.instance.CurrenPlayer;
				Data data = formatter.Deserialize(stream) as Data;

				// money
				player.Money = data.Money;
				player.AddMoney(0);

				// inventory items
				player.Inventory.InstantiatePrefabs(data);

				// equiped items
				player.Equipment.InstantiatePrefabs(data);

				// stats
				player.Stats.Level = data.Level;
				player.Stats.CurrentHealth = data.CurrentHealth;
				player.Stats.Exp = data.Exp;
				player.Stats.ExpForLevelUp = data.ExpForLevelUp;
			}

			return true;
		}

		return false;
	}
}
