using UnityEngine;
using System.Collections.Generic;

public class EnemyController: UnitController
{
    // Properties //
    public int distanceOfSight = 20;

	// Functions //
	protected override void Start()
	{
		base.Start();

		Inventory = Instantiate(inventoryPrefab);
		Inventory.transform.SetParent(windowsRoot.transform, false);
		Inventory.Owner = this;
		Inventory.name = "Inv + Equip (" + transform.name + ")";

		Equipment = Inventory.GetComponent<Equipment>();
		Equipment.Owner = this;
	}

	void Update()
    {
        if (state == State.Waiting)
        {
            return;
        }
        else if (CurrentActionPoints == 0)
        {
            EndTurn();
        }

		else if (state == State.ReadingInput)
		{
			TryToAim();
		}
		else if (state == State.Moving)
		{
			MakeStep(true);
		}
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
			state = State.Moving;
		}
		else
		{
			if (WasteActionPoints(weapon.ActionPointsForUse) == true)
			{
				LookAt(unitInFocus.transform.position);
				UnitAnim.Idle(false);
				weapon.Fire();

				state = State.Waiting;
			}
			else
			{
				EndTurn();
			}
		}
	}

	public override void CalculatePossibleTiles(int _distance = 0)
	{
		// calculating distance of sight and not just tiles that we can step on,
		// so the map of the possible moves will be bigger than our action points can handle.
		// we are trying to detect our target so we need more possible tiles
		// and limit of our movement will be controlled by action points

		base.CalculatePossibleTiles(distanceOfSight);
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

    PlayerController GetFirstTarget()
    {
        foreach (UnitController unit in GameManager.instance.units)
        {
            if (unit.tag == "Player")
            {
                return unit.GetComponent<PlayerController>();
            }
        }

        return null;
    }

    bool GetTileForMelee(UnitController target, out Tile tileForMelee)
    {
        Tile closestTile = grid.tiles[0, 0];
        float closestDistance = 999;
        List<Tile> targetAdjacentTiles = target.grid.GetClosestTile(target.transform.position).GetAdjacentTiles();

        foreach (Tile tile in targetAdjacentTiles)
        {
            float distance = Vector3.Distance(transform.position, tile.spawnPoint);

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
        Tile myTile = grid.GetClosestTile(transform.position);
        Tile targetTile = grid.GetClosestTile(GetFirstTarget().transform.position);
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
}