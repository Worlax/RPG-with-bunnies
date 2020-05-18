using UnityEngine;

public class InventorySlot: MonoBehaviour
{
    // Properties //
	[ReadOnly]
    public Item itemInSlot;

    // Functions //
	// return false to give back dragged item (failed connect or swap)
    public virtual bool ConnectOrSwapItem(Item draggedItem, bool ignoreMoveTurn = false)
    {
        if (ignoreMoveTurn == false && GameManager.instance.playerMove == false)
        {
            return false;
        }

        if (itemInSlot == null)
        {
            ConnectItem(draggedItem);
        }
        else
        {
            // if new item came for equipment slot
            if (draggedItem.lastConnectedSlot is EquipmentSlot && itemInSlot is Equippable)
			{
				Equippable equippable1 = itemInSlot as Equippable;
				Equippable equippable2 = draggedItem as Equippable;

				if (equippable1.SlotType == equippable2.SlotType)
                {
                    SwapItems(draggedItem);
                }
                else
                {
                    return false;
                }
            }
            // if item in slot and dragged item are StackableItems
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
            // if item in slot is gun and dragged item is ammo
            else if (itemInSlot is Gun && draggedItem as Ammo)
            {
                Gun gun = itemInSlot as Gun;
                Ammo ammo = draggedItem as Ammo;

                if (gun.AmmoName == ammo.itemName)
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
}
