using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Inventory: Window
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] RectTransform slotsRoot;
	[SerializeField] Text moneyText;

#pragma warning restore 0649

	[SerializeField] Item[] startItems;

	// Functions //
	protected override void Start()
	{
		base.Start();

		InstantiatePrefabs();
	}

	public void Init()
	{
		Owner.OnMoneyChanged += MoneyChanged;
		MoneyChanged();
	}

	void OnDisable()
	{
		Owner.OnMoneyChanged -= MoneyChanged;
	}

	public override void Open()
	{
		if (Owner.InBattle == false)
		{
			base.Open();
		}
	}

	void InstantiatePrefabs()
	{
		for (int i = 0; i < startItems.Length; ++i)
		{
			if (startItems[i] != null)
			{
				Item item = Instantiate(startItems[i]);
				item.transform.SetParent(slotsRoot.GetChild(i), false);
			}
		}
	}

	void OnValidate()
	{
		int slotsCount = GetAllSlots().Length;

		if (startItems.Length > slotsCount)
		{
			print("Size of default items is limited by amount of slots.");
			Array.Resize(ref startItems, slotsCount);
		}
	}

	InventorySlot[] GetAllSlots(RectTransform root = null)
	{
		List<InventorySlot> slots = new List<InventorySlot>();

		if (root == null)
		{
			root = slotsRoot;
		}

		foreach (Transform transform in root)
		{
			InventorySlot slot = transform.GetComponent<InventorySlot>();

			if (slot != null)
				slots.Add(slot);
		}

		return slots.ToArray();
	}

	void MoneyChanged()
	{
		string display = Owner.Money.ToString();

		for (int i = display.Length, j = 0; i > 0; --i, ++j)
		{
			if (j != 0 && j % 3 == 0)
			{
				display = display.Insert(i, " ");
			}
		}

		moneyText.text = display;
	}

	int CountEmptySlots(RectTransform root = null)
	{
		int emptySlots = 0;

		foreach (InventorySlot slot in GetAllSlots(root))
		{
			if (slot.IsEmpty())
			{
				++emptySlots;
			}
		}

		return emptySlots;
	}

	InventorySlot GetFirstEmptySlot(RectTransform root = null)
	{
		foreach (InventorySlot slot in GetAllSlots(root))
		{
			if (slot.IsEmpty())
				return slot;
		}

		return null;
	}

	public Item[] GetAllItems(RectTransform root = null)
	{
		List<Item> items = new List<Item>();

		foreach (InventorySlot slot in GetAllSlots(root))
		{
			if (slot.IsEmpty() == false)
			{
				items.Add(slot.GetItem());
			}
		}

		return items.ToArray();
	}

	public bool TryToConnectItems(Item[] items)
	{
		if (CountEmptySlots() < items.Length)
		{
			return false;
		}
		else
		{
			InventorySlot[] slots = GetAllSlots();
			int i = 0;

			foreach (Item item in items)
			{
				while (slots[i].IsEmpty() == false)
				{
					++i;
				}

				slots[i].ConnectOrSwapItem(item, true);
			}

			return true;
		}
	}
}
