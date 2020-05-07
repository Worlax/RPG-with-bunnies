using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlot: InventorySlot
{
    // Properties //
    public EquipmentItem.Type type;
    GameObject itemIn3D;

    // Events //

    public static event Action<Item2D, Item3D> OnItemEquiped;
    public static event Action<Item2D, Item3D> OnItemUnequiped;

    // Functions //
    public override bool ConnectOrSwapItem(Item2D newItem, bool ignoreMoveTurn = false)
    {
        EquipmentItem equipmentItem = newItem as EquipmentItem;
        if (equipmentItem == null || equipmentItem.type != type)
            return false;

        return base.ConnectOrSwapItem(newItem, ignoreMoveTurn);
    }

    protected override void ConnectItem(Item2D newItem)
    {
        base.ConnectItem(newItem);
        EquipItem();
    }

    public override void DisconnectItem()
    {
        UnequipItem();
        base.DisconnectItem();
    }

    protected override void SwapItems(Item2D newItem)
    {
        base.SwapItems(newItem);
    }

    public void EquipItem()
    {
        PlayerController player = GameManager.instance.currentUnit as PlayerController;

        itemIn3D = Instantiate(itemInSlot.ItemPrefab);
        itemIn3D.transform.SetParent(player.transform, false);

        if (type == EquipmentItem.Type.Weapon)
        {
            Weapon weapon = itemIn3D.GetComponent<Weapon>();

            weapon.holder = player;
            player.weapon = weapon;

            // load ammo info from buffer
            Gun gun = itemIn3D.GetComponent<Gun>();
            EquipmentItem _itemInSlot = itemInSlot as EquipmentItem;

            if (gun != null && _itemInSlot != null)
            {
                gun.magazineCurrentAmmo = _itemInSlot.magazineCurrentAmmo;
            }
        }
        
        OnItemEquiped(itemInSlot, itemIn3D.GetComponent<Item3D>());
    }

    public void UnequipItem()
    {
        if (type == EquipmentItem.Type.Weapon)
        {
            PlayerController player = GameManager.instance.currentUnit as PlayerController;
            Weapon weapon = itemIn3D.GetComponent<Weapon>();

            weapon.holder = null;
            player.weapon = null;

            // buffer ammo info
            Gun gun = itemIn3D.GetComponent<Gun>();
            EquipmentItem _itemInSlot = itemInSlot as EquipmentItem;

            if (gun != null && _itemInSlot != null)
            {
                _itemInSlot.magazineCurrentAmmo = gun.magazineCurrentAmmo;
            }
        }
        
        OnItemUnequiped(itemInSlot, itemIn3D.GetComponent<Item3D>());

        Destroy(itemIn3D);
    }
}
