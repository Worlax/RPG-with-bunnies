using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot: MonoBehaviour
{
    // Properties //
    public Item itemInSlot;

    // Functions //
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
            if (draggedItem.lastConnectedSlot is EquipmentSlot)
            {
                bool check = FromEquipmentSlotCheck(draggedItem as Equippable);

                if (check == true)
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
            else if (itemInSlot is Gun2 && draggedItem as Ammo)
            {
                Gun2 gun = itemInSlot as Gun2;
                Ammo ammo = draggedItem as Ammo;

                if (gun.ammoName == ammo.itemName)
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

    bool FromEquipmentSlotCheck(Equippable draggedItem)
    {
        Equippable equipmentItemInLocalSlot = itemInSlot as Equippable;

        if (equipmentItemInLocalSlot != null && equipmentItemInLocalSlot.type == draggedItem.type)
        {
            return true;
        }

        return false;
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

        int spaceInStack = localItem.maxInStack - localItem.inStack;
        if (spaceInStack > 0)
        {
            if (spaceInStack >= draggedItem.inStack)
            {
                localItem.AddToStack(draggedItem.inStack);

                Destroy(draggedItem.gameObject);

                return true;
            }
            else
            {
                localItem.AddToStack(spaceInStack);
                draggedItem.SubtractFromStack(spaceInStack);
            }
        }

        return false;
    }

    bool LoadWeapon(Ammo ammo)
    {
        Gun2 gun = itemInSlot as Gun2;
        int ammoMissing = gun.maxAmmo - gun.currentAmmo;

        if (ammoMissing > 0)
        {
            // take some ammo from stack
            if (ammoMissing < ammo.inStack)
            {
				gun.maxAmmo += ammoMissing;
                ammo.SubtractFromStack(ammoMissing);

                return false;
            }
            // take all ammo
            else
            {
				gun.currentAmmo += ammo.inStack;
                Destroy(ammo.gameObject);

                return true;
            }
        }

        return false;
    }
}
