using UnityEngine;
using System;
using System.Collections;

public class Weapon2: Item
{
    // Properties //
    protected LineRenderer hitLine;
    public UnitController holder;

    public int actionPointsForUse = 2;
    public float hitDistance = 6;
    public int minDamage = 3;
    public int maxDamage = 5;

    protected Animator animator;
    public bool fireAnimationInProcess = false;

    public float animationTimeBeforHit = 0.83f;
    public float animationTimeAfterHit = 1.46f;

    // Events //
    public event Action FireAnimationComplete;

    public UnitController AimedTarget { get; private set; }

    // Functions //
    protected override void EquipItemEffect()
    {
        base.EquipItemEffect();

        hitLine = GameManager.instance.WeaponAimLineRenderer;
        animator = itemIn3D.GetComponentInChildren<Animator>();
    }

    protected override void UnequipItemEffect()
    {
        base.UnequipItemEffect();


    }

    public virtual void Aim(UnitController target)
    {
        holder.GetComponent<Animator>().SetBool("aiming", true);

        LayerMask mask = LayerMask.GetMask("Tile") | LayerMask.GetMask("Item");

        Physics.Raycast(holder.transform.position, (target.transform.position - holder.transform.position).normalized, out RaycastHit hit, hitDistance, ~mask);

        // raycast hit
        if (hit.transform != null)
        {
            // hit target
            if (hit.transform.GetComponent<UnitController>() != null)
            {
                DrawHitLine(hit.point, false);

                AimedTarget = target;
                AimedTarget.Targeted();
            }
            // hit other object
            else
            {
                DrawHitLine(hit.point, false);
            }
        }
        // hit no object
        else
        {  
            DrawHitLine(target.transform.position, true);
        }
    }

    void DrawHitLine(Vector3 targetPosition, bool useDistance)
    {
        if (hitLine == null)
        {
            hitLine = GameManager.instance.WeaponAimLineRenderer;
        }

        Vector3 firstPoint = holder.transform.position;
        Vector3 secondPoint;

        if (useDistance)
        {
            secondPoint = firstPoint + ((targetPosition - holder.transform.position).normalized) * hitDistance;
        }
        else
        {
            secondPoint = targetPosition;
        }

        hitLine.positionCount = 2;
        hitLine.SetPosition(0, firstPoint);
        hitLine.SetPosition(1, secondPoint);
    }

    public virtual void StopAim()
    {
        holder.GetComponent<Animator>().SetBool("aiming", false);

        hitLine.positionCount = 0;

        if (AimedTarget != null)
        {
            AimedTarget.Untargeted();
            AimedTarget = null;
        }
    }

    public virtual bool Fire(UnitController target, int fireTimes = 1)
    {
        if (fireAnimationInProcess == true)
        {
            return false;
        }
        if (holder.WasteActionPoints(actionPointsForUse) == false)
        {
            return false;
        }

        StartCoroutine(FireAnimation(target, fireTimes));
        return true;
    }

    IEnumerator FireAnimation(UnitController target, int playTimes)
    {
        for (int i = 0; i < playTimes; ++i)
        {
            holder.LookAt(target.transform.position);

            animator.Play("Fire", 0, 0);
            fireAnimationInProcess = true;
            yield return new WaitForSeconds(animationTimeBeforHit);

            FireEvent(target);

            yield return new WaitForSeconds(animationTimeAfterHit);
        }

        fireAnimationInProcess = false;
        FireAnimationComplete?.Invoke();
    }

    protected virtual void FireEvent(UnitController target)
    {
        Stats targetStats = target.GetComponent<Stats>();
        targetStats.DealDamage(UnityEngine.Random.Range(minDamage, maxDamage + 1), holder);

        // for player
        PlayerController player = holder.GetComponent<PlayerController>();

        if (targetStats.dead == true && player != null)
        {
            player.DefocusTarget();
        }
    }
}
