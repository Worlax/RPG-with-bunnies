using UnityEngine;
using System;
using System.Collections.Generic;

[SelectionBase]
public class UnitController : MonoBehaviour
{
	// Properties //
	protected UnitAnim unitAnim;

    public GameGrid grid;
    public float speed = 2;
    public const int maxActionPoints = 10;
    public int startActionPoints = 5;
    public int currentActionPoints = 0;
    protected const int MaxReservedActionPoints = 3;
    protected int reservedActionPoints = 0;

    Vector3 targetPosition = new Vector3();
    Vector3 direction = new Vector3();

    protected Stack<Tile> movePath = new Stack<Tile>();

	public Weapon weapon;

	public Transform windowsRoot;

	// Events //
	public static event Action<UnitController> OnNewUnitTurn;
    public static event Action<UnitController> OnActionPointsChanged;
    public static event Action<UnitController> OnUnitEndTurn;

    // State //
    public enum State
    {
        NotMyMove,
        RoundStart,
        ReadingInput,
        CalculatingStep,
        Moving,
        Waiting
    }

    public State state = State.NotMyMove;

    // Functions //
    protected virtual void Start()
    {
        transform.position = grid.GetClosestTile(transform.position).spawnPoint;
		unitAnim = GetComponent<UnitAnim>();
		currentActionPoints = startActionPoints;

		unitAnim.Idle(true);
	}

    protected virtual void Update()
    {

	}

    public void StartRound()
    {
        currentActionPoints = startActionPoints + reservedActionPoints;
        if (currentActionPoints > maxActionPoints)
        {
            currentActionPoints = maxActionPoints;
        }

        OnNewUnitTurn?.Invoke(this);
        state = State.RoundStart;
    }

    // Unit movement //
    public void CalculatePossibleMoves(int _distance = 0)
    {
        int distance;

        if (_distance > 0)
        {
            distance = _distance;
        }
        else
        {
            distance = currentActionPoints;
        }

        ClearInfo();
       
        Tile currentTile = grid.GetClosestTile(transform.position);
        Queue<Tile> tilesQueue = new Queue<Tile>();
        currentTile.SetStateContainsUnit();
        grid.FindAllBlocked();

        tilesQueue.Enqueue(currentTile);
        while (tilesQueue.Count > 0)
        {
            Tile inProcess = tilesQueue.Dequeue();
            foreach (Tile tile in inProcess.GetAdjacentTiles())
            {
                if (tile && tile.state != Tile.State.Blocked && tile.state != Tile.State.ContainsUnit && tile.bChecked == false && inProcess.numberOfParents < distance)
                {
                    tile.SetStatePossible();
                    tile.parent = inProcess;
                    tile.numberOfParents = inProcess.numberOfParents + 1;
                    tile.bChecked = true;

                    tilesQueue.Enqueue(tile);
                }
            }
        }
    }

    protected bool CalculateStep()
    {
        if (movePath.Count > 0)
        {
            targetPosition = movePath.Pop().spawnPoint;
            direction = (targetPosition - transform.position).normalized;

            LookAt(targetPosition);
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void MakeStep()
    {
		unitAnim.Idle(false);

        transform.position += direction * Time.deltaTime * speed;

        if (Vector3.Distance(targetPosition, transform.position) <= 0.10f)
        {
            transform.position = targetPosition;
            WasteActionPoints(1, false);
            state = State.CalculatingStep;

			unitAnim.Idle(true);
		}
    }

    public void LookAt(Vector3 target)
    {
        transform.LookAt(target);
    }

    protected void CalculatePath(Tile toTile)
    {
        Tile currentTile = toTile;
        movePath.Push(currentTile);

        for (int i = 0; i < toTile.numberOfParents - 1; ++i)
        {
            currentTile = currentTile.parent;
            movePath.Push(currentTile);
        }
    }

    // Overlap //
    public void Targeted()
    {
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
    }

    public void Untargeted()
    {
        GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
    }

    //....//
    public bool WasteActionPoints(int AP, bool RecalculatePossibleMoves = true)
    {
        if (currentActionPoints >= AP)
        {
            currentActionPoints -= AP;
            if (RecalculatePossibleMoves) { CalculatePossibleMoves(); }
            OnActionPointsChanged?.Invoke(this);

            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void WaitTurn()
    {

    }

    public virtual void EndTurn()
    {
        reservedActionPoints = currentActionPoints;
        if (reservedActionPoints > MaxReservedActionPoints)
        {
            reservedActionPoints = MaxReservedActionPoints;
        }

        ClearInfo();
        state = State.NotMyMove;
        OnUnitEndTurn?.Invoke(this);
    }

    protected virtual void ClearInfo()
    {
        grid.ResetAll();
        movePath.Clear();
    }

    public virtual void KillUnit()
    {
		if (weapon != null)
		{
			weapon.ItemVisual.GetComponentInChildren<Animated>().Disable();
			weapon.ItemVisual.transform.parent = null;
		}

		GetComponent<Animator>().Play("Death");
    }
}