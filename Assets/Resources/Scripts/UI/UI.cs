using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Button wait;
	[SerializeField] Button skip;

	[SerializeField] Button fireMode;
	[SerializeField] RectTransform ammo;

#pragma warning restore 0649

	// Functions //
	void Start()
    {
        wait.onClick.AddListener(WaitTurn);
        skip.onClick.AddListener(EndTurn);
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

	public void WaitTurn()
    {
		if (BattleManager.instance.PlayerMove == false || BattleManager.instance.CurrentUnit.battleState != UnitController.BattleState.ReadingInput)
			return;

		BattleManager.instance.CurrentUnit.GetComponent<PlayerController>().WaitTurn();
    }

    public void EndTurn()
    {
        if (BattleManager.instance.PlayerMove == false || BattleManager.instance.CurrentUnit.battleState != UnitController.BattleState.ReadingInput)
            return;

        BattleManager.instance.CurrentUnit.GetComponent<PlayerController>().EndTurn();
    }

	void WeaponEquiped(Weapon weapon)
	{
		Gun gun = weapon as Gun;

		if (gun != null)
		{
			fireMode.GetComponentInChildren<Text>().text = gun.GetFireModeName();
			UpdateAmmoDisplay(gun);

			ammo.transform.localScale = Vector3.one;
			fireMode.transform.localScale = Vector3.one;

			gun.AmmoChanged += UpdateAmmoDisplay;
		}
	}

	void WeaponUnequiped(Weapon weapon)
	{
		Gun gun = weapon as Gun;

		if (gun != null)
		{
			ammo.transform.localScale = Vector3.zero;
			fireMode.transform.localScale = Vector3.zero;

			gun.AmmoChanged -= UpdateAmmoDisplay;
		}
	}

	void UpdateAmmoDisplay(Gun gun)
	{
		ammo.GetComponentInChildren<Text>().text = gun.CurrentAmmo.ToString() + " \\ " + gun.MaxAmmo.ToString();
	}

	public void ToggleFireMode()
	{
		if (BattleManager.instance.PlayerMove == false)
			return;

		PlayerController player = BattleManager.instance.CurrentUnit.GetComponent<PlayerController>();

		Gun gun = player.weapon as Gun;
		if (gun == null)
			return;

		gun.SwitchFireMode();
		fireMode.GetComponentInChildren<Text>().text = gun.GetFireModeName();
	}
}
