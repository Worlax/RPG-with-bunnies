using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    // Singleton //
    public static ItemsManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //

        Dollar.MaxInStack = dollar;

        Ammo_7D62x51mm.MaxInStack = ammo_7D62x51mm;
        Ammo_5D56x45mm.MaxInStack = ammo_5D56x45mm;
        Ammo_9mm.MaxInStack = ammo_9mm;

        Rifle.magazineMaxAmmo = rifle;

        HealingPotion.MaxUses = healingPotion;
    }

    // Properties //
    [Header("MaxInStack")]
    public int dollar = 1000;

    public int ammo_7D62x51mm = 50;
    public int ammo_5D56x45mm = 80;
    public int ammo_9mm = 120;

    [Header("MaxAmmoInMagazine")]
    public int rifle = 30;

    [Header("MaxUses")]
    public int healingPotion = 3;
	
    // Functions //

}
