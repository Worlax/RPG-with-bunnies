using UnityEngine;
using System;
using System.Collections;

public class Weapon: Equippable
{
	// Properties //
	protected WeaponAnim weaponAnim;

	protected LineRenderer hitLine;
    public UnitController holder;

	public bool randomLocationForDamagePopup = false;

	public int actionPointsForUse = 2;
    public float hitDistance = 6;
    public int minDamage = 3;
    public int maxDamage = 5;

    public UnitController AimedTarget { get; private set; }
	protected Vector3 hitLocation;

	public enum Stage
	{
		Idle,
		Aiming,
		Firing
	}

	public Stage stage;

    // Functions //
    public override void EquipItem(UnitController ownerOfThisItem)
    {
		base.EquipItem(ownerOfThisItem);

		hitLine = GameManager.instance.WeaponAimLineRenderer;
		holder = ownerOfThisItem;
		ownerOfThisItem.weapon = this;
		weaponAnim = ItemVisual.GetComponentInChildren<WeaponAnim>();

		if (ownerOfThisItem.GetComponent<Stats>().dead)
		{
			weaponAnim.Disable();
			ItemVisual.transform.parent = null;
		}
	}

	public override void UnequipItem()
    {
		PlayerController player = GameManager.instance.currentUnit as PlayerController;

		weaponAnim = null;
		holder = null;
		player.weapon = null;

		base.UnequipItem();
	}

	public virtual void Aim(UnitController target)
    {
		weaponAnim.Aim(target.transform);

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

    void DrawHitLine(Vector3 _secondPoint, bool useDistance)
    {
        Vector3 firstPoint = holder.transform.position;
        Vector3 secondPoint;

        if (useDistance)
        {
            secondPoint = firstPoint + ((_secondPoint - holder.transform.position).normalized) * hitDistance;
        }
        else
        {
            secondPoint = _secondPoint;
        }

        hitLine.positionCount = 2;
        hitLine.SetPosition(0, firstPoint);
        hitLine.SetPosition(1, secondPoint);

		hitLocation = hitLine.GetPosition(1);
	}

    public virtual void StopAim()
    {
		weaponAnim.StopAim();

        hitLine.positionCount = 0;
		hitLocation = Vector3.zero;

		if (AimedTarget != null)
        {
            AimedTarget.Untargeted();
            AimedTarget = null;
        }
    }

    public virtual void Fire()
    {
		weaponAnim.Fire(hitLocation, this);
    }

	public virtual void WeaponFired()
	{
		return;
	}

	public virtual void WeaponHit()
	{
		if (AimedTarget == null)
			return;

		Stats targetStats = AimedTarget.GetComponent<Stats>();
		int damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);

		targetStats.DealDamage(holder, damage, randomLocationForDamagePopup);
	}
}
