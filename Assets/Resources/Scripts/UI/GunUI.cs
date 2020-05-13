using UnityEngine;
using UnityEngine.UI;

public class GunUI: MonoBehaviour
{
    // Properties //
    public Button fireMode;
    public RectTransform ammoUI;

    // Functions //
    void Start()
    {
        fireMode.onClick.AddListener(ToggleFireMode);
    }

    void OnEnable()
    {
		EquipmentSlot.PlayerEquipedWeapon += WeaponEquiped;
		EquipmentSlot.PlayerUnequipedWeapon += WeaponUnequiped;
    }

    void OnDisable()
    {
		EquipmentSlot.PlayerEquipedWeapon -= WeaponEquiped;
		EquipmentSlot.PlayerUnequipedWeapon -= WeaponUnequiped;
	}

    void WeaponEquiped(Weapon weapon)
    {
        Gun gun = weapon as Gun;

        if (gun != null)
        {
            fireMode.GetComponentInChildren<Text>().text = gun.GetFireModeName();
            UpdateAmmoDisplay(gun);

            ammoUI.transform.localScale = Vector3.one;
            fireMode.transform.localScale = Vector3.one;

            gun.AmmoChanged += UpdateAmmoDisplay;
        }
    }

    void WeaponUnequiped(Weapon weapon)
	{
        Gun gun = weapon as Gun;

        if (gun != null)
        {
            ammoUI.transform.localScale = Vector3.zero;
            fireMode.transform.localScale = Vector3.zero;

            gun.AmmoChanged -= UpdateAmmoDisplay;
        }
    }

    void UpdateAmmoDisplay(Gun gun)
    {
        ammoUI.GetComponentInChildren<Text>().text = gun.currentAmmo.ToString() + " \\ " + gun.maxAmmo.ToString();
    }

    public void ToggleFireMode()
    {
        if (GameManager.instance.playerMove == false)
            return;

        PlayerController player = GameManager.instance.currentUnit.GetComponent<PlayerController>();

        Gun gun = player.weapon as Gun;
        if (gun == null)
            return;

        gun.SwitchFireMode();
        fireMode.GetComponentInChildren<Text>().text = gun.GetFireModeName();
    }
}
