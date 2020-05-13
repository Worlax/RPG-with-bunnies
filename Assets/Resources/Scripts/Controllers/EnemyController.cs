using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController: UnitController
{
    // Properties //
    public int distanceOfSight = 20;
    PlayerController targetPlayer;
    Tile targetTile;

    bool inMeleeRange = false;

    bool damageAnimationInProcess = false;

    float animationTimeBeforHit = 0.83f;
    float animationTimeAfterHit = 1.46f;

	public Inventory inventoryPrefab;
	Equipment equipment;
	Inventory inventory;

	// Functions //
	protected override void Start()
	{
		base.Start();

		inventory = Instantiate(inventoryPrefab);
		inventory.transform.SetParent(canvas.transform, false);
		inventory.Owner = this;
		inventory.name = "Inventory (" + transform.name + ")";

		equipment = inventory.GetComponent<Equipment>();
		equipment.Owner = this;
	}

	protected override void Update()
    {
        base.Update();

        if (state == State.NotMyMove)
        {
            return;
        }

        if (currentActionPoints == 0 && damageAnimationInProcess == false)
        {
            EndTurn();
        }
        else if (inMeleeRange)
        {
            if (IsInMeleeRange())
            {
                StartDamageAnimation();
            }
            else
            {
                inMeleeRange = false;
            }
        }

        if (state == State.RoundStart)
        {
            // calculating distance of sight and not just tiles that we can step on,
            // so the map of the possible moves will be bigger than our action points can handle.
            // we are trying to detect our target so we need more possible tiles
            // and limit of our movement will be controlled by action points

            CalculatePossibleMoves(distanceOfSight);
            state = State.ReadingInput;
        }
        else if (state == State.ReadingInput)
        {
            if (CreatingPath())
            {
                state = State.CalculatingStep;
            }
            else
            {
                EndTurn();
            }
        }
        else if (state == State.CalculatingStep)
        {
            if (IsInMeleeRange())
            {
                inMeleeRange = true;
            }
            else if (CalculateStep())
            {
                state = State.Moving;
            }
            else if (currentActionPoints > 0)
            {
                state = State.RoundStart;
            }
        }
        else if (state == State.Moving)
        {
            MakeStep();
        }
    }

    bool CreatingPath()
    {
        targetPlayer = GetFirstTarget();
        Tile targetTile;

        if (GetTileForMelee(targetPlayer, out targetTile))
        {
            CalculatePath(targetTile);
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

    bool GetTileForMelee(PlayerController target, out Tile tileForMelee)
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

    void StartDamageAnimation()
    {
        if (damageAnimationInProcess == false)
        {
            if (WasteActionPoints(2))
            {
                damageAnimationInProcess = true;

                PlayerController target = GetFirstTarget();
                StartCoroutine(DealDamage(target, 5));
            }
            else
            {
                EndTurn();
            }
        }    
    }

    IEnumerator DealDamage(PlayerController target, int damage)
    {
        LookAt(target.transform.position);
        GetComponentsInChildren<Animator>()[1].Play("Fire");
        yield return new WaitForSeconds(animationTimeBeforHit);
        
        target.GetComponent<Stats>().DealDamage(this, damage);
        target.FocusTarget(this);
        yield return new WaitForSeconds(animationTimeAfterHit);

        damageAnimationInProcess = false;
    }

    public override void EndTurn()
    {
        if (damageAnimationInProcess == true)
            return;

        base.EndTurn();

        damageAnimationInProcess = false;
    }
}