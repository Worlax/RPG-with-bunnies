using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot: MonoBehaviour
{
    // Properties //
    public Item2D itemInSlot;

    // Functions //
    public virtual bool ConnectOrSwapItem(Item2D newItem, bool ignoreMoveTurn = false)
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
            if (newItem.GetLastConnectedSlot() is EquipmentSlot)
            {
                bool check = FromEquipmentSlotCheck(newItem as EquipmentItem);

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
            else if (itemInSlot is StackableItem && newItem is StackableItem)
            {
                bool check = StackableItemCheck(newItem as StackableItem);

                if (check == true)
                {
                    return CombineStackable(newItem as StackableItem);
                }
                else
                {
                    SwapItems(newItem);
                }  
            }
            // if new item is ammo and item in slot is gun
            else if (itemInSlot.ItemPrefab.GetComponent<Gun>() != null && newItem.GetComponent<Ammo>() != null)
            {
                EquipmentItem equipmentItem = itemInSlot as EquipmentItem;
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

    bool FromEquipmentSlotCheck(EquipmentItem newItem)
    {
        EquipmentItem equipmentItemInLocalSlot = itemInSlot as EquipmentItem;

        if (equipmentItemInLocalSlot != null && equipmentItemInLocalSlot.type == newItem.type)
        {
            return true;
        }

        return false;
    }

    bool StackableItemCheck(StackableItem newItem)
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

    protected virtual void ConnectItem(Item2D newItem)
    {
        itemInSlot = newItem;
        itemInSlot.SetLastConnectedSlot(this);
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

    protected virtual void SwapItems(Item2D newItem)
    {
        InventorySlot secondSlot = newItem.GetLastConnectedSlot();
        Item2D localItem = itemInSlot;

        DisconnectItem();

        ConnectItem(newItem);
        secondSlot.ConnectItem(localItem);
    }

    bool CombineStackable(StackableItem newItem)
    {
        StackableItem localItem = itemInSlot as StackableItem;

        int spaceInStack = StackableItem.maxInStack - localItem.inStack;
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
        EquipmentItem equipmentItem = itemInSlot as EquipmentItem;
        int lackOfAmmo = equipmentItem.magazineMaxAmmo - equipmentItem.magazineCurrentAmmo;

        if (lackOfAmmo > 0)
        {
            // take some ammo from stack
            if (lackOfAmmo < ammo.inStack)
            {
                equipmentItem.magazineCurrentAmmo += lackOfAmmo;
                ammo.Subtract(lackOfAmmo);

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
