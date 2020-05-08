using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot: MonoBehaviour
{
    // Properties //
    public Item itemInSlot;

    // Functions //
    public virtual bool ConnectOrSwapItem(Item newItem, bool ignoreMoveTurn = false)
    {
        if (ignoreMoveTurn == false && GameManager.instance.playerMove == false)
        {
            return false;
        }

        if (itemInSlot == null)
        {
            ConnectItem(newItem);
        }
        else
        {
            // if newItem came from EquipmentSlot
            if (newItem.lastConnectedSlot is EquipmentSlot)
            {
                bool check = FromEquipmentSlotCheck(newItem as Equippable);

                if (check == true)
                {
                    SwapItems(newItem);
                }
                else
                {
                    return false;
                }
            }
            // if newItem and itemInSlot are StackableItems
            else if (itemInSlot is Stackable && newItem is Stackable)
            {
                bool check = StackableItemCheck(newItem as Stackable);

                if (check == true)
                {
                    return CombineStackable(newItem as Stackable);
                }
                else
                {
                    SwapItems(newItem);
                }  
            }
            // if new item is ammo and item in slot is gun
            else if (itemInSlot.itemIn3DPrefab.GetComponent<Gun>() != null && newItem.GetComponent<Ammo>() != null)
            {
                Equippable equipmentItem = itemInSlot as Equippable;
                Ammo ammo = newItem.GetComponent<Ammo>();

                if (equipmentItem.GetAmmoType() == ammo.GetType())
                {
                    return LoadWeapon(ammo);
                }

                return false;
            }
            else
            {
                SwapItems(newItem);
            }
        }

        return true;
    }

    bool FromEquipmentSlotCheck(Equippable newItem)
    {
        Equippable equipmentItemInLocalSlot = itemInSlot as Equippable;

        if (equipmentItemInLocalSlot != null && equipmentItemInLocalSlot.type == newItem.type)
        {
            return true;
        }

        return false;
    }

    bool StackableItemCheck(Item newItem)
    {
        if (itemInSlot.GetType() == newItem.GetType())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual void ConnectItem(Item newItem)
    {
        itemInSlot = newItem;
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

    protected virtual void SwapItems(Item newItem)
    {
        InventorySlot secondSlot = newItem.lastConnectedSlot;
        Item localItem = itemInSlot;

        DisconnectItem();

        ConnectItem(newItem);
        secondSlot.ConnectItem(localItem);
    }

    bool CombineStackable(Stackable newItem)
    {
        Stackable localItem = itemInSlot as Stackable;

        int spaceInStack = Stackable.maxInStack - localItem.inStack;
        if (spaceInStack > 0)
        {
            if (spaceInStack >= newItem.inStack)
            {
                localItem.Add(newItem.inStack);

                Destroy(newItem.gameObject);

                return true;
            }
            else
            {
                localItem.Add(spaceInStack);
                newItem.Subtract(spaceInStack);
            }
        }

        return false;
    }

    bool LoadWeapon(Ammo ammo)
    {
        Equippable equipmentItem = itemInSlot as Equippable;
        int lackOfAmmo = equipmentItem.magazineMaxAmmo - equipmentItem.magazineCurrentAmmo;

        if (lackOfAmmo > 0)
        {
            // take some ammo from stack
            if (lackOfAmmo < ammo.inStack)
            {
                equipmentItem.magazineCurrentAmmo += lackOfAmmo;
                ammo.SubtractFromStack(lackOfAmmo);

                return false;
            }
            // take all ammo
            else
            {
                equipmentItem.magazineCurrentAmmo += ammo.inStack;
                Destroy(ammo.gameObject);

                return true;
            }
        }

        return false;
    }
}
