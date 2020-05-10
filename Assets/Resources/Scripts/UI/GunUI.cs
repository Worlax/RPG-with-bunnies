using UnityEngine;
using UnityEngine.UI;

public class GunUI: MonoBehaviour
{
    // Properties //
    public Button fireMode;
    public RectTransform ammoDisplay;

    // Functions //
    void Start()
    {
        fireMode.onClick.AddListener(ToggleFireMode);
    }

    void OnEnable()
    {
		Equippable.OnItemEquiped += ItemEquiped;
		Equippable.OnItemUnequiped += ItemUnequiped;
    }

    void OnDisable()
    {
		Equippable.OnItemEquiped -= ItemEquiped;
		Equippable.OnItemUnequiped -= ItemUnequiped;
    }

    void ItemEquiped(Equippable item)
    {
        Gun gun = item as Gun;

        if (gun != null)
        {
            fireMode.GetComponentInChildren<Text>().text = gun.GetFireModeName();
            UpdateAmmoDisplay(gun);

            ammoDisplay.transform.localScale = Vector3.one;
            fireMode.transform.localScale = Vector3.one;

            gun.AmmoChanged += UpdateAmmoDisplay;
        }
    }

    void ItemUnequiped(Equippable item)
    {
        Gun gun = item as Gun;

        if (gun != null)
        {
            ammoDisplay.transform.localScale = Vector3.zero;
            fireMode.transform.localScale = Vector3.zero;

            gun.AmmoChanged -= UpdateAmmoDisplay;
        }
    }

    void UpdateAmmoDisplay(Gun gun)
    {
        ammoDisplay.GetComponentInChildren<Text>().text = gun.currentAmmo.ToString() + " \\ " + gun.maxAmmo.ToString();
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
