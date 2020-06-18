using UnityEngine;
using UnityEngine.UI;
using System;

public class Gun : Weapon
{
	// Properties //
	GunAnim gunAnim;

	Text ammoText;

	[Header("Single")]
	[SerializeField] bool sAvailable = true;
	[SerializeField] int sActionPoints = 2;
	[SerializeField] int sAmmo = 1;
	[SerializeField] int sMinDamage = 7;
	[SerializeField] int sMaxDamage = 10;

	[Header("Burst")]
	[SerializeField] bool bAvailable = true;
	[SerializeField] int bActionPoints = 3;
	[SerializeField] int bAmmo = 3;
	[SerializeField] int bMinDamage = 6;
	[SerializeField] int bMaxDamage = 8;

	[Header("Automatic")]
	[SerializeField] bool aAvailable = true;
	[SerializeField] int aActionPoints = 4;
	[SerializeField] int aAmmo = 6;
	[SerializeField] int aMinDamage = 5;
	[SerializeField] int aMaxDamage = 6;

	[Header("Ammo")]
	[SerializeField] Ammo.AmmoType _AmmoType;
	[SerializeField] int _currentAmmo;
	[SerializeField] int _maxAmmo = 30;

	public Ammo.AmmoType AmmoType { get => _AmmoType; private set => _AmmoType = value; }
	public int CurrentAmmo { get => _currentAmmo; set => _currentAmmo = value; }
	public int MaxAmmo { get => _maxAmmo; set => _maxAmmo = value; }

	int _ammoForUse;
	public int AmmoForUse { get => _ammoForUse; private set => _ammoForUse = value; }

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

	protected override void Start()
	{
		base.Start();

		UpdateVisual();
	}

	public override void EquipItem(Unit ownerOfThisItem)
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
		base.Fire();

		if (CurrentAmmo >= AmmoForUse)
		{
			gunAnim.Fire(AimedTarget.transform, hitLocation, AmmoForUse);
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

		if (holder is Player)
		{
			(holder as Player).Equipment.UpdateStatsDisplay();
		}
    }

    bool IsFireModeAvaible()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                return sAvailable;

            case FireMode.Burst:
                return bAvailable;

            case FireMode.Automatic:
                return aAvailable;

            default:
                return false;
        }
    }

    void UpdateWeaponInfo()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                ActionPointsForUse = sActionPoints;
                MinDamage = sMinDamage;
                MaxDamage = sMaxDamage;
                AmmoForUse = sAmmo;
				randomLocationForDamagePopup = false;
				break;

            case FireMode.Burst:
                ActionPointsForUse = bActionPoints;
                MinDamage = bMinDamage;
                MaxDamage = bMaxDamage;
                AmmoForUse = bAmmo;
				randomLocationForDamagePopup = true;
				break;

            case FireMode.Automatic:
                ActionPointsForUse = aActionPoints;
                MinDamage = aMinDamage;
                MaxDamage = aMaxDamage;
                AmmoForUse = aAmmo;
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

		UpdateVisual();
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

		UpdateVisual();
		AmmoChanged?.Invoke(this);

		return returnAmmo;
    }

	void UpdateVisual()
	{
		ammoText.text = CurrentAmmo.ToString();
	}
}
