using UnityEngine;
using System.Collections.Generic;

public class Bag: Inventory
{
    // Properties //
    public List<InventorySlot> slots;
    public List<Item2D> itemsPrefabs;

    // Functions //
    protected override void Start()
    {
        base.Start();
        UpdateItems();
    }

    void UpdateItems()
    {
        for (int i = 0; i < itemsPrefabs.Count; ++i)
        {
            if (i > slots.Count)
                return;

            Item2D item = Instantiate(itemsPrefabs[i]);

            item.transform.SetParent(slots[i].transform, false);
        }
    }
}
