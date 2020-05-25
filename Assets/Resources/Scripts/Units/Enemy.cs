using UnityEngine;
using System.Collections.Generic;

public class Enemy: Unit
{
	// Properties //
#pragma warning disable 0649

	public bool isPatroling;
	[SerializeField] float timeBetweenPatrolSteps = 4f;

#pragma warning restore 0649

	Patrol patrol;

	int distanceOfSight = 50;

	float enterTime;

	// Functions //
	protected override void Awake()
	{
		base.Awake();

		patrol = GetComponent<Patrol>();
	}

	protected override void Start()
	{
		base.Start();

		Inventory = Instantiate(InventoryPrefab);
		Inventory.transform.SetParent(WindowsRoot.transform, false);
		Inventory.Owner = this;
		Inventory.name = "Inv + Equip (" + transform.name + ")";
		Inventory.Init();
		Inventory.Close();

		Equipment = Inventory.GetComponent<Equipment>();
		Equipment.Owner = this;
	}

	void Update()
    {
		if (!InBattle)
		{
			// patrol logic
			if (isPatroling && battleState == BattleState.Waiting)
			{
				if (enterTime == 0)
				{
					enterTime = Time.time;
				}
				else if (Time.time - enterTime >= timeBetweenPatrolSteps)
				{
					enterTime = -1;
					Patrol();
				}
			}
			else if (isPatroling && battleState == BattleState.Moving)
			{
				MakeStep(false, false, BattleState.Waiting);
			}
		}
		else if (InBattle && !MyTurn)
		{
			return;
		}
        else if (InBattle && MyTurn && CurrentActionPoints == 0)
        {
			EndTurn();
        }

		// battle logic
		else if (battleState == BattleState.ReadingInput)
		{
			TryToAim();
		}
		else if (battleState == BattleState.Moving)
		{
			if (InBattle)
			{
				MakeStep(true);
			}
		}
	}

	public override void StartBattle()
	{
		base.StartBattle();

		isPatroling = false;
	}

	public override void StartRound()
	{
		base.StartRound();

		if (CreatingPath() == false)
		{
			EndTurn();
			return;
		}
	}

	void TryToAim()
	{
		if (weapon == null)
		{
			EndTurn();
			return;
		}

		weapon.Aim(unitInFocus);
		
		if (weapon.AimedTarget == null)
		{
			weapon.StopAim();
			battleState = BattleState.Moving;
		}
		else
		{
			if (weapon != null && weapon is Gun)
			{
				if ((weapon as Gun).CurrentAmmo == 0)
				{
					EndTurn();
				}
			}
			if (WasteActionPoints(weapon.ActionPointsForUse) == true)
			{
				LookAt(unitInFocus.transform.position);
				UnitAnim.Idle(false);
				weapon.Fire();

				battleState = BattleState.Waiting;
			}
			else
			{
				EndTurn();
			}
		}
	}

	public override void CalculatePossibleTiles(int _distance = 0, bool flagPossibleTiles = true)
	{
		// calculating distance of sight and not just tiles that we can step on,
		// so the map of the possible moves will be bigger than our action points can handle.
		// we are trying to detect our target so we need more possible tiles
		// and limit of our movement will be controlled by action points

		base.CalculatePossibleTiles(distanceOfSight, flagPossibleTiles);
	}

	bool CreatingPath()
    {
		FocusTarget(GetFirstTarget());
        Tile targetTile;

        if (GetTileForMelee(unitInFocus, out targetTile))
        {
            CalculateAllSteps(targetTile);
            return true;
        }
        else
        {
            return false;
        }
    }

    Player GetFirstTarget()
    {
        foreach (Unit unit in BattleManager.instance.BattlingUnits)
        {
            if (unit.tag == "Player")
            {
                return unit.GetComponent<Player>();
            }
        }

        return null;
    }

    bool GetTileForMelee(Unit target, out Tile tileForMelee)
    {
        Tile closestTile = Grid.Tiles[0, 0];
        float closestDistance = 999;

        List<Tile> targetAdjacentTiles = Grid.GetClosestTile(target.transform.position).GetAdjacentTiles();

        foreach (Tile tile in targetAdjacentTiles)
        {
            float distance = Vector3.Distance(transform.position, tile.SpawnPoint);

            if (distance < closestDistance && tile.state == Tile.State.Possible)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        tileForMelee = closestTile;

        if (closestDistance == 999f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool IsInMeleeRange()
    {
        Tile myTile = Grid.GetClosestTile(transform.position);
        Tile targetTile = Grid.GetClosestTile(GetFirstTarget().transform.position);
        List<Tile> tilesForMelee = targetTile.GetAdjacentTiles();

        foreach (Tile tile in tilesForMelee)
        {
            if (myTile == tile)
            {
                return true;
            }
        }

        return false;
    }

	void Patrol()
	{
		movePath = patrol.CalculateAllSteps();
		CalculateNextStep();

		battleState = BattleState.Moving;
		enterTime = 0;
	}
}