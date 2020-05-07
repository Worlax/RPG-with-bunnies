using UnityEngine;

public class EquipmentItem : Item2D
{
    // Properties //
    public enum Type
    {
        Helmet,
        Body,
        Weapon
    }

    public Type type = Type.Weapon;

    public enum AmmoType
    {
        A_5D56x45mm,
        A_7D62x51mm,
        A_9mm
    }

    public AmmoType ammoType;
    public int magazineMaxAmmo;
    public int magazineCurrentAmmo;
    public int magazineSpawnAmmo = 25;

    // Functions //
    protected override void Start()
    {
        base.Start();

        Gun gun = ItemPrefab.GetComponent<Gun>();

        if (gun != null)
        {
            magazineCurrentAmmo = magazineSpawnAmmo;
            magazineMaxAmmo = gun.GetMaxAmmo();
        }
    }

    public System.Type GetAmmoType()
    {
        switch (ammoType)
        {
            case AmmoType.A_5D56x45mm:
                return typeof(Ammo_5D56x45mm);

            case AmmoType.A_7D62x51mm:
                return typeof(Ammo_7D62x51mm);

            case AmmoType.A_9mm:
                return typeof(Ammo_9mm);

            default:
                return null;
        }
    }
}
