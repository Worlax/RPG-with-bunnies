using UnityEngine;
using UnityEngine.UI;
using System;

public class Gun: Weapon
{
	// Properties //
	GunAnim gunAnim;

	Text ammoText;

	[SerializeField]
	bool singleAvailable = true;
	[SerializeField]
	bool burstAvailable = true;
	[SerializeField]
	bool automaticAvailable = true;

	[SerializeField]
	int singleActionPoints = 2;
	[SerializeField]
	int burstActionPoints = 3;
	[SerializeField]
	int automaticActionPoints = 4;

	[SerializeField]
	int singleAmmo = 1;
	[SerializeField]
	int burstAmmo = 3;
	[SerializeField]
	int automaticAmmo = 6;

	[SerializeField]
	int singleMinDamage = 7;
	[SerializeField]
	int singleMaxDamage = 10;
	[SerializeField]
	int burstMinDamage = 6;
	[SerializeField]
	int burstMaxDamage = 8;
	[SerializeField]
	int automaticMinDamage = 5;
	[SerializeField]
	int automaticMaxDamage = 6;

	[SerializeField]
	string _ammoName;
    public string AmmoName { get => _ammoName; set => _ammoName = value; }
	[SerializeField]
	int _currentAmmo;
    public int CurrentAmmo { get => _currentAmmo; set => _currentAmmo = value; }
	[SerializeField]
	int _maxAmmo = 30;
    public int MaxAmmo { get => _maxAmmo; set => _maxAmmo = value; }

	protected int ammoForUse;

	public enum FireMode
    {
        Single,
        Burst,
        Automatic
    }

	protected FireMode fireMode = FireMode.Single;

	// Events //
	public event Action<Gun> AmmoChanged;

    // Functions //
    void Awake()
    {
        UpdateWeaponInfo();
        CurrentAmmo = MaxAmmo;

		ammoText = GetComponentInChildren<Text>();
		ammoText.text = CurrentAmmo.ToString();
	}

	public override void EquipItem(UnitController ownerOfThisItem)
	{
		base.EquipItem(ownerOfThisItem);

		gunAnim = ItemVisual.GetComponentInChildren<GunAnim>();
		gunAnim.Weapon = this;
	}

	public override void UnequipItem()
	{
		base.UnequipItem();

		gunAnim.Weapon = null;
		gunAnim = null;
	}

	public override void Fire()
    {
		if (CurrentAmmo >= ammoForUse)
		{
			gunAnim.Fire(AimedTarget.transform, hitLocation, ammoForUse);
		}
	}

	public void BulletFired()
	{
		TakeAmmo(1);
	}
	
	public void SwitchFireMode()
    {
        Stats holderStats = holder.GetComponent<Stats>();
        holderStats.SubtractStats(0, MinDamage, MaxDamage);

        int fireModeElements = System.Enum.GetNames(typeof(FireMode)).Length;

        // if "fireMode" set with last element of the class
        if (fireModeElements - 1 == (int)fireMode)
        {
            fireMode = 0;
        }
        else
        {
            ++fireMode;
        }

        while (IsFireModeAvaible() == false)
        {
            if (fireModeElements - 1 == (int)fireMode)
            {
                fireMode = 0;
            }
            else
            {
                ++fireMode;
            }
        }

        UpdateWeaponInfo();
        holderStats.AddStats(0, MinDamage, MaxDamage);

		if (holder is PlayerController)
		{
			(holder as PlayerController).Equipment.UpdateStatsDisplay();
		}
    }

    bool IsFireModeAvaible()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                return singleAvailable;

            case FireMode.Burst:
                return burstAvailable;

            case FireMode.Automatic:
                return automaticAvailable;

            default:
                return false;
        }
    }

    void UpdateWeaponInfo()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                ActionPointsForUse = singleActionPoints;
                MinDamage = singleMinDamage;
                MaxDamage = singleMaxDamage;
                ammoForUse = singleAmmo;
				randomLocationForDamagePopup = false;
				break;

            case FireMode.Burst:
                ActionPointsForUse = burstActionPoints;
                MinDamage = burstMinDamage;
                MaxDamage = burstMaxDamage;
                ammoForUse = burstAmmo;
				randomLocationForDamagePopup = true;
				break;

            case FireMode.Automatic:
                ActionPointsForUse = automaticActionPoints;
                MinDamage = automaticMinDamage;
                MaxDamage = automaticMaxDamage;
                ammoForUse = automaticAmmo;
				randomLocationForDamagePopup = true;
				break;
        }
    }

    public string GetFireModeName()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                return "Single";

            case FireMode.Burst:
                return "Burst";

            case FireMode.Automatic:
                return "Auto";

            default:
                return "None";
        }
    }

    public void AddAmmo(int amount)
    {
        CurrentAmmo += amount;

        if (CurrentAmmo > MaxAmmo)
        {
            CurrentAmmo = MaxAmmo;
        }

		ammoText.text = CurrentAmmo.ToString();
		AmmoChanged?.Invoke(this);
    }

    public int TakeAmmo(int amount)
    {
		int returnAmmo = 0;

		if (CurrentAmmo >= amount)
		{
			returnAmmo = amount;
			CurrentAmmo -= amount;
		}
		else
		{
			returnAmmo = CurrentAmmo;
			CurrentAmmo = 0;
		}

		ammoText.text = CurrentAmmo.ToString();
		AmmoChanged?.Invoke(this);

		return returnAmmo;
    }
}
