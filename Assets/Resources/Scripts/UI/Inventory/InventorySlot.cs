using UnityEngine;

public class InventorySlot: MonoBehaviour
{
    // Properties //
	[ReadOnly] public Item itemInSlot;

    // Functions //
	// return false to give back dragged item (failed connect or swap)
    public virtual bool ConnectOrSwapItem(Item draggedItem)
    {
		// Money
		if (draggedItem is Dollar)
		{
			Inventory inventory = GetComponentInParent<Inventory>();
			
			if (inventory != null && inventory.Owner is Player)
			{
				Dollar moneyItem = draggedItem as Dollar;
				int moneyTaken;

				moneyItem.TakeFromStack(moneyItem.inStack, out moneyTaken);
				inventory.Owner.AddMoney(moneyTaken);
			}
		}
		// connect
        if (itemInSlot == null)
        {
			// item came from shop
			if (draggedItem.lastConnectedSlot is ShopSlot)
			{
				if (this is ShopSlot)
				{
					// do nothing
				}
				else
				{
					ShopSlot shopSlot = draggedItem.lastConnectedSlot as ShopSlot;

					if (shopSlot.SlotType != ShopSlot.Type.Sell)
						return false;
				}	
			}

			ConnectItem(draggedItem);
        }
		// swap
        else
        {
			// item came from shop
			if (draggedItem.lastConnectedSlot is ShopSlot)
			{
				if (this is ShopSlot)
				{
					// do nothing
				}
				else
				{
					ShopSlot shopSlot = draggedItem.lastConnectedSlot as ShopSlot;

					if (shopSlot.SlotType != ShopSlot.Type.Sell)
						return false;
				}
			}

			// item came from equipment slot
			if (draggedItem.lastConnectedSlot is EquipmentSlot)
			{
				Equippable equippable1 = itemInSlot as Equippable;
				Equippable equippable2 = draggedItem as Equippable;

				if (equippable1 != null && equippable1.SlotType == equippable2.SlotType)
                {
                    SwapItems(draggedItem);
                }
                else
                {
                    return false;
                }
            }
            // item in slot and dragged item are StackableItems
            else if (itemInSlot is Stackable && draggedItem is Stackable)
            {
                if (itemInSlot.itemName == draggedItem.itemName)
                {
                    return CombineStackable(draggedItem as Stackable);
                }
                else
                {
                    SwapItems(draggedItem);
                }  
            }
            // item in slot is gun and dragged item is ammo
            else if (itemInSlot is Gun && draggedItem as Ammo)
            {
                Gun gun = itemInSlot as Gun;
                Ammo ammo = draggedItem as Ammo;

                if (gun.AmmoType == ammo.Type)
                {
                    return LoadWeapon(ammo);
                }

                return false;
            }
            else
            {
                SwapItems(draggedItem);
            }
        }

        return true;
    }

	public void ConnectOrSwapItem(Item draggedItem, bool forceConnection)
	{
		if (draggedItem.lastConnectedSlot != null)
		{
			draggedItem.lastConnectedSlot.DisconnectItem();
		}

		ConnectItem(draggedItem);
	}


	protected virtual void ConnectItem(Item draggedItem)
    {
		itemInSlot = draggedItem;
        itemInSlot.lastConnectedSlot = this;
        itemInSlot.transform.SetParent(transform);
        itemInSlot.transform.localPosition = Vector3.zero;
    }

    public virtual void DisconnectItem()
    {
        if (itemInSlot == null)
            return;

        itemInSlot.transform.SetParent(GetComponentInParent<Canvas>().transform);
        itemInSlot = null;
    }

    protected virtual void SwapItems(Item draggedItem)
    {
        InventorySlot secondSlot = draggedItem.lastConnectedSlot;
        Item localItem = itemInSlot;

        DisconnectItem();

        ConnectItem(draggedItem);
        secondSlot.ConnectItem(localItem);
    }

    bool CombineStackable(Stackable draggedItem)
    {
        Stackable localItem = itemInSlot as Stackable;
        int spaceInLocalStack = localItem.maxInStack - localItem.inStack;

		draggedItem.TakeFromStack(spaceInLocalStack, out int wasTaken);
		localItem.AddToStack(wasTaken);

		if (draggedItem == null)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    bool LoadWeapon(Ammo ammo)
    {
        Gun gun = itemInSlot as Gun;
        int ammoMissing = gun.MaxAmmo - gun.CurrentAmmo;

		ammo.TakeFromStack(ammoMissing, out int ammoWasTaken);
		gun.AddAmmo(ammoWasTaken);

		if (ammo == null)
		{
			return true;
		}
		else
		{
			return false;
		}
    }

	public bool IsEmpty()
	{
		foreach (Transform trans in transform)
		{
			if (trans.GetComponent<Item>() != null)
				return false;
		}

		return true;
	}

	public Item GetItem()
	{
		foreach (Transform trans in transform)
		{
			Item item = trans.GetComponent<Item>();
			if (item != null)
				return item;
		}

		return null;
	}
}
