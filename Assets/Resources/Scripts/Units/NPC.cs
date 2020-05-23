using UnityEngine;

public class NPC: Unit
{
	// Properties //


	// Functions //
	protected override void Start()
	{
		base.Start();

		Inventory = Instantiate(InventoryPrefab);
		Inventory.transform.SetParent(WindowsRoot.transform, false);
		Inventory.Owner = this;
		Inventory.name = "Shop (" + transform.name + ")";
		Inventory.Init();
		Inventory.Close();
	}
}
