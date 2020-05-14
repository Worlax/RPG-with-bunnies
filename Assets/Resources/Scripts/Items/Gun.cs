using UnityEngine.UI;
using System;

public class Gun: Weapon
{
	// Properties //
	GunAnim gunAnim;

	Text ammoText;

    public bool singleAvailable = true;
    public bool burstAvailable = true;
	public bool automaticAvailable = true;

    public int singleActionPoints = 2;
    public int burstActionPoints = 3;
    public int automaticActionPoints = 4;

    public int singleAmmo = 1;
    public int burstAmmo = 3;
    public int automaticAmmo = 6;
    public int ammoForUse;

    public int singleMinDamage = 7;
    public int singleMaxDamage = 10;
    public int burstMinDamage = 6;
    public int burstMaxDamage = 8;
    public int automaticMinDamage = 5;
    public int automaticMaxDamage = 6;

    public string ammoName;
    public int maxAmmo;
    public int currentAmmo;

    public enum FireMode
    {
        Single,
        Burst,
        Automatic
    }

    public FireMode fireMode = FireMode.Single;

	// Events //
	public event Action<Gun> AmmoChanged;

    // Functions //
    void Awake()
    {
        UpdateWeaponInfo();
        currentAmmo = maxAmmo;

		ammoText = GetComponentInChildren<Text>();
		ammoText.text = currentAmmo.ToString();
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
		if (currentAmmo >= ammoForUse)
		{
			gunAnim.Fire(hitLocation, ammoForUse);
		}
	}

	public void BulletFired()
	{
		TakeAmmo(1);
	}
	
	public void SwitchFireMode()
    {
        Stats holderStats = holder.GetComponent<Stats>();
        holderStats.SubtractStats(0, minDamage, maxDamage);

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
        holderStats.AddStats(0, minDamage, maxDamage);

		if (holder is PlayerController)
		{
			(holder as PlayerController).MyEquipment.UpdateStatsDisplay();
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
                actionPointsForUse = singleActionPoints;
                minDamage = singleMinDamage;
                maxDamage = singleMaxDamage;
                ammoForUse = singleAmmo;
				randomLocationForDamagePopup = false;
				break;

            case FireMode.Burst:
                actionPointsForUse = burstActionPoints;
                minDamage = burstMinDamage;
                maxDamage = burstMaxDamage;
                ammoForUse = burstAmmo;
				randomLocationForDamagePopup = true;
				break;

            case FireMode.Automatic:
                actionPointsForUse = automaticActionPoints;
                minDamage = automaticMinDamage;
                maxDamage = automaticMaxDamage;
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
        currentAmmo += amount;

        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }

		ammoText.text = currentAmmo.ToString();
		AmmoChanged?.Invoke(this);
    }

    public int TakeAmmo(int amount)
    {
		int returnAmmo = 0;

		if (currentAmmo >= amount)
		{
			returnAmmo = amount;
			currentAmmo -= amount;
		}
		else
		{
			returnAmmo = currentAmmo;
			currentAmmo = 0;
		}

		ammoText.text = currentAmmo.ToString();
		AmmoChanged?.Invoke(this);

		return returnAmmo;
    }
}
