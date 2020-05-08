using UnityEngine;
using System;
using System.Collections;

public class Gun: Weapon
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

    protected string aimAnimationName = "Aim";
    protected string stopAimAnimationName = "StopAim";
    public bool AimAnimationInProcess = false;
    public float aimAnimationTime = 1.35f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    Vector3 hitPosition;

    public static int magazineMaxAmmo = 30;
    public int magazineCurrentAmmo;

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
        SetCurrentProperties();
        magazineCurrentAmmo = magazineMaxAmmo;
    }

    public override bool Fire(UnitController target, int fireTimes)
    {
        int _ammoForUse = ammoForUse;

        if (_ammoForUse > magazineCurrentAmmo)
            return false;

        if (AimAnimationInProcess == true)
            return false;

        if (base.Fire(target, ammoForUse) == false)
            return false;

        magazineCurrentAmmo -= _ammoForUse;
        AmmoChanged?.Invoke(this);
        return true;
    }

    protected override void FireEvent(UnitController target)
    {
        transform.LookAt(target.transform);

        Bullet bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
        bullet.transform.rotation = transform.rotation;
        bullet.transform.position = firePoint.position;

        Vector3 direction = (target.transform.position - bullet.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

        Stats targetStats = target.GetComponent<Stats>();

        bullet.Fire(hitPosition, targetStats, UnityEngine.Random.Range(minDamage, maxDamage + 1), holder);
    }

    public override void Aim(UnitController target)
    { 
        if (AimAnimationInProcess == true)
            return;

        StartCoroutine(AimAnimation());

        base.Aim(target);
        hitPosition = hitLine.GetPosition(1);
    }

    IEnumerator AimAnimation()
    {
        animator.SetBool("aiming", true);
        AimAnimationInProcess = true;
        yield return new WaitForSeconds(aimAnimationTime);

        AimAnimationInProcess = false;
    }

    public override void StopAim()
    {
        if (state == State.Normal)
            return;

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

        SetCurrentProperties();
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

    void SetCurrentProperties()
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
                return "Burst"; // test 2

            case FireMode.Automatic:
                return "Auto";

            default:
                return "None";
        }
    }

    public virtual int GetMaxAmmo()
    {
        return Gun.magazineMaxAmmo;
    }

    public void AddAmmo(int ammo)
    {
        magazineCurrentAmmo += ammo;

        if (magazineCurrentAmmo > GetMaxAmmo())
        {
            magazineCurrentAmmo = GetMaxAmmo();
        }

        AmmoChanged?.Invoke(this);
    }

    public void Reload()
    {


        AmmoChanged?.Invoke(this);
    }
}
