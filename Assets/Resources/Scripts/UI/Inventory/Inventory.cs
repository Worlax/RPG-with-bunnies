using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory: Window
{
	// Properties //
	int slotCount;
	[SerializeField] RectTransform slotsRoot = default;

	[SerializeField] Item[] startItems;

	// Functions //
	protected override void Start()
	{
		base.Start();

		InstantiatePrefabs();
	}

	void OnValidate()
	{
		CountSlots();

		if (startItems.Length > slotCount)
		{
			print("Size of default items is limited by amount of slots.");
			Array.Resize(ref startItems, slotCount);
		}
	}

	void CountSlots()
	{
		slotCount = 0;

		foreach (Transform obj in slotsRoot)
		{
			InventorySlot slot = obj.GetComponent<InventorySlot>();
			if (slot != null)
			{
				++slotCount;
			}
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
}
