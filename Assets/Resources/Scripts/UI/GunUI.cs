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
        EquipmentSlot.OnItemEquiped += ItemEquiped;
        EquipmentSlot.OnItemUnequiped += ItemUnequiped;
    }

    void OnDisable()
    {
        EquipmentSlot.OnItemEquiped -= ItemEquiped;
        EquipmentSlot.OnItemUnequiped -= ItemUnequiped;
    }

    void ItemEquiped(Item2D item, Item3D ItemIn3d)
    {
        Gun gun = ItemIn3d as Gun;

        if (gun != null)
        {
            fireMode.GetComponentInChildren<Text>().text = gun.GetFireModeName();
            UpdateAmmoDisplay(gun);

            ammoDisplay.transform.localScale = Vector3.one;
            fireMode.transform.localScale = Vector3.one;

            gun.AmmoChanged += UpdateAmmoDisplay;
        }
    }

    void ItemUnequiped(Item2D item, Item3D ItemIn3d)
    {
        Gun gun = ItemIn3d as Gun;

        if (gun != null)
        {
            ammoDisplay.transform.localScale = Vector3.zero;
            fireMode.transform.localScale = Vector3.zero;

            gun.AmmoChanged -= UpdateAmmoDisplay;
        }
    }

    void UpdateAmmoDisplay(Gun gun)
    {
        ammoDisplay.GetComponentInChildren<Text>().text = gun.magazineCurrentAmmo.ToString() + " \\ " + gun.GetMaxAmmo().ToString();
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
