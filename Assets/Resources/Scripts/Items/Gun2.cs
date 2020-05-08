using UnityEngine;
using System;
using System.Collections;

public class Gun2: Weapon2
{
    // Properties //
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

    public bool aimAnimationInProcess = false;
    public float aimAnimationTime = 1.35f;

    public GameObject bulletPrefab;
    Vector3 firePoint;
    public float bulletSpeed = 20f;

    Vector3 hitPosition;

    static int maxAmmo;
    public static int MaxAmmo
    {
        get => maxAmmo;

        set
        {
            if (maxAmmo == 0) { maxAmmo = value; }
            else { print("Trying to set property twice"); }
        }
    }

    public int currentAmmo;

    public enum FireMode
    {
        Single,
        Burst,
        Automatic
    }

    public FireMode fireMode = FireMode.Single;

    // Events //
    public event Action<Gun2> AmmoChanged;

    // Functions //
    void Awake()
    {
        UpdateFireInfo();
        currentAmmo = maxAmmo;
    }

    protected override void EquipItemEffect()
    {
        base.EquipItemEffect();

        firePoint = itemIn3D.transform.GetChild(0).transform.position;
    }

    protected override void UnequipItemEffect()
    {
        base.UnequipItemEffect();


    }

    public override bool Fire(UnitController target, int fireTimes)
    {
        int _ammoForUse = ammoForUse;

        if (_ammoForUse > currentAmmo)
            return false;

        if (aimAnimationInProcess == true)
            return false;

        if (base.Fire(target, ammoForUse) == false)
            return false;

        currentAmmo -= _ammoForUse;
        AmmoChanged?.Invoke(this);
        return true;
    }

    protected override void FireEvent(UnitController target)
    {
        transform.LookAt(target.transform);

        Bullet bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
        bullet.transform.rotation = transform.rotation;
        bullet.transform.position = firePoint;

        Vector3 direction = (target.transform.position - bullet.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

        Stats targetStats = target.GetComponent<Stats>();

        bullet.Fire(hitPosition, targetStats, UnityEngine.Random.Range(minDamage, maxDamage + 1), holder);
    }

    public override void Aim(UnitController target)
    {
        if (aimAnimationInProcess == true)
            return;

        StartCoroutine(AimAnimation());

        base.Aim(target);
        hitPosition = hitLine.GetPosition(1);
    }

    IEnumerator AimAnimation()
    {
        animator.SetBool("aiming", true);
        aimAnimationInProcess = true;
        yield return new WaitForSeconds(aimAnimationTime);

        aimAnimationInProcess = false;
    }

    public override void StopAim()
    {
        animator.SetBool("aiming", false);

        base.StopAim();
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

        UpdateFireInfo();
        holderStats.AddStats(0, minDamage, maxDamage);
        GameManager.instance.equipment.UpdateStatsDisplay();
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

    void UpdateFireInfo()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                actionPointsForUse = singleActionPoints;
                minDamage = singleMinDamage;
                maxDamage = singleMaxDamage;
                ammoForUse = singleAmmo;
                break;

            case FireMode.Burst:
                actionPointsForUse = burstActionPoints;
                minDamage = burstMinDamage;
                maxDamage = burstMaxDamage;
                ammoForUse = burstAmmo;
                break;

            case FireMode.Automatic:
                actionPointsForUse = automaticActionPoints;
                minDamage = automaticMinDamage;
                maxDamage = automaticMaxDamage;
                ammoForUse = automaticAmmo;
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

        if (currentAmmo > MaxAmmo)
        {
            currentAmmo = MaxAmmo;
        }

        AmmoChanged?.Invoke(this);
    }

    public void SubtractAmmo(int amount)
    {
        currentAmmo -= amount;

        if (currentAmmo < 0)
        {
            currentAmmo = 0;
        }

        AmmoChanged?.Invoke(this);
    }
}
