using UnityEngine;

public abstract class Weapon: Equippable
{
	// Properties //
	[ReadOnly][SerializeField] protected Unit holder;

	[SerializeField] int _actionPointsForUse = 2;
	[SerializeField] float _hitDistance = 6;
	[SerializeField] int _minDamage = 3;
	[SerializeField] int _maxDamage = 5;

	public int ActionPointsForUse { get => _actionPointsForUse; protected set => _actionPointsForUse = value; }
	public float HitDistance { get => _hitDistance; protected set => _hitDistance = value; }
	public int MinDamage { get => _minDamage; protected set => _minDamage = value; }
	public int MaxDamage { get => _maxDamage; protected set => _maxDamage = value; }

	WeaponAnim weaponAnim;
	protected LineRenderer hitLine;

	public Unit AimedTarget { get; private set; }

	protected Vector3 hitLocation;
	protected bool randomLocationForDamagePopup = false;

	public bool firing { get; private set; }

    // Functions //
	void Update()
	{
		if (AimedTarget != null)
		{
			holder.LookAt(AimedTarget.transform);
			Aim(AimedTarget);
		}
	}

    public override void EquipItem(Unit ownerOfThisItem)
    {
		base.EquipItem(ownerOfThisItem);

		hitLine = BattleManager.instance.WeaponAimLineRenderer;
		holder = ownerOfThisItem;
		ownerOfThisItem.weapon = this;
		weaponAnim = ItemVisual.GetComponentInChildren<WeaponAnim>();

		if (ownerOfThisItem.GetComponent<Stats>().Dead)
		{
			weaponAnim.Disable();
			ItemVisual.transform.parent = null;
		}
	}

	public override void UnequipItem()
    {
		weaponAnim = null;
		holder.weapon = null;
		holder = null;

		base.UnequipItem();
	}

	public virtual void Aim(Unit target)
    {
		weaponAnim.Aim(target.transform);

        LayerMask mask = LayerMask.GetMask("Tile") | LayerMask.GetMask("Item") | LayerMask.GetMask("Ignore Raycast");
        Physics.Raycast(holder.transform.position, (target.transform.position - holder.transform.position).normalized, out RaycastHit hit, HitDistance, ~mask);

        // raycast hit
        if (hit.transform != null)
        {
			// hit target
			Unit hitedUnit = hit.transform.GetComponentInParent<Unit>();
			if (hitedUnit != null && hitedUnit == target)
            {
                DrawHitLine(hit.point, false);

                AimedTarget = target;
                AimedTarget.Targeted();
            }
            // hit other object
            else
            {
				if (AimedTarget == null)
				{
					DrawHitLine(hit.point, false);
				}
                else
				{
					StopAim();
				}
            }
        }
        // hit no object
        else
        {
			if (AimedTarget == null)
			{
				DrawHitLine(target.transform.position, true);
			}
			else
			{
				StopAim();
			}
		}
    }

    void DrawHitLine(Vector3 _secondPoint, bool useDistance)
    {
        Vector3 firstPoint = holder.transform.position;
        Vector3 secondPoint;

        if (useDistance)
        {
            secondPoint = firstPoint + ((_secondPoint - holder.transform.position).normalized) * HitDistance;
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
		firing = true;
	}

	public virtual void WeaponHit()
	{
		if (AimedTarget == null)
			return;

		Stats targetStats = AimedTarget.GetComponent<Stats>();
		int damage = UnityEngine.Random.Range(holder.Stats.MinDamage, holder.Stats.MaxDamage + 1);

		targetStats.DealDamage(holder, damage, randomLocationForDamagePopup);
	}

	public virtual void WeaponFired()
	{
		firing = false;
		holder.battleState = Unit.BattleState.ReadingInput;
	}
}
