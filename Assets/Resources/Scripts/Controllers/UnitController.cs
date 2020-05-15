using UnityEngine;
using System;
using System.Collections.Generic;

[SelectionBase]
public abstract class UnitController: MonoBehaviour, IComparable<UnitController>
{
	// Properties //
	public bool MyTurn { get; protected set; } = false;

	protected UnitAnim unitAnim;

	public UnitController unitInFocus;

	public GameGrid grid;
    public float speed = 2;
    public float animSpeed = 2;
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

    // State //
    public enum State
    {
        Waiting,
        ReadingInput,
        Moving
    }

    public State state = State.Waiting;

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
		return;
	}

	public int CompareTo(UnitController obj)
	{
		return obj.speed.CompareTo(speed);
	}

	public virtual void StartRound()
    {
		print(name + " turn.");

		currentActionPoints = startActionPoints + reservedActionPoints;
        if (currentActionPoints > maxActionPoints)
        {
            currentActionPoints = maxActionPoints;
        }

		CalculatePossibleTiles();
		OnNewUnitTurn?.Invoke(this);

		MyTurn = true;
		state = State.ReadingInput;
    }

    // Unit movement //
    public virtual void CalculatePossibleTiles(int _distance = 0)
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

	protected void CalculateAllSteps(Tile finaleTile)
	{
		Tile currentTile = finaleTile;
		movePath.Push(currentTile);

		for (int i = 0; i < finaleTile.numberOfParents - 1; ++i)
		{
			currentTile = currentTile.parent;
			movePath.Push(currentTile);
		}

		CalculateNextStep();
	}

	protected bool CalculateNextStep()
    {
        if (movePath.Count > 0)
        {
            targetPosition = movePath.Pop().spawnPoint;
            direction = (targetPosition - transform.position).normalized;

			return true;
        }
        else
        {
            return false;
        }
    }

    protected void MakeStep(bool stopAfterOneTile = false)
    {
		unitAnim.Idle(false);

		LookAt(targetPosition);
		transform.position += direction * Time.deltaTime * animSpeed;

        if (Vector3.Distance(targetPosition, transform.position) <= 0.10f)
        {
            transform.position = targetPosition;
            WasteActionPoints(1, false);

			if (CalculateNextStep() == false)
			{
				CalculatePossibleTiles();
				state = State.ReadingInput;
				unitAnim.Idle(true);
			}

			if (stopAfterOneTile == true)
			{
				state = State.ReadingInput;
			}
		}
    }

    public void LookAt(Vector3 target)
    {
        transform.LookAt(target);
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
            if (RecalculatePossibleMoves) { CalculatePossibleTiles(); }
            OnActionPointsChanged?.Invoke(this);

            return true;
        }
        else
        {
            return false;
        }
    }

	public virtual void FocusTarget(UnitController target)
	{
		unitInFocus = target;
	}

	public virtual void DefocusTarget()
	{
		unitInFocus = null;

		if (weapon != null)
		{
			weapon.StopAim();
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

		unitAnim.Idle(true);
		DefocusTarget();
		ClearInfo();

		MyTurn = false;
		state = State.Waiting;
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
		MyTurn = false;
    }
}