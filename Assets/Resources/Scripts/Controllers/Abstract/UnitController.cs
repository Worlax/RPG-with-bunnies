using UnityEngine;
using System;
using System.Collections.Generic;

[SelectionBase]
public abstract class UnitController: MonoBehaviour, IComparable<UnitController>
{
	// Properties //
	// inspector
	[SerializeField] Transform _visual;
	[SerializeField] GameGrid _grid;
	[SerializeField] Inventory _inventoryPrefab;
	[SerializeField] Transform _windowsRoot;
	[SerializeField] [Range(0.5f, 5f)] float _speed = 1.5f;
	[SerializeField] [Range(0, 10)] int _startActionPoints = 5;
	[ReadOnly] public Weapon weapon;
	[ReadOnly] public UnitController unitInFocus;

	public Transform Visual { get => _visual; }
	public GameGrid Grid { get => _grid; }
	public Inventory InventoryPrefab { get => _inventoryPrefab; }
	public Transform WindowsRoot { get => _windowsRoot; }
	public float Speed { get => _speed; }
	public int StartActionPoints { get => _startActionPoints; }

	//
	public Inventory Inventory { get; protected set; }
	public Equipment Equipment { get; protected set; }

	public const int maxActionPoints = 10;
	public const int maxReservedActionPoints = 3;
	public int CurrentActionPoints { get; protected set; }
	public int ReservedActionPoints { get; protected set; }

	public Stats Stats { get; private set; }
	public UnitAnim UnitAnim { get; private set; }
	public bool MyTurn { get; protected set; } = false;

	protected Stack<Tile> movePath = new Stack<Tile>();
	Vector3 nextTilePosition;
	Vector3 nextTiledirection;

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

	[HideInInspector]
	public State state = State.Waiting;

	// Functions //
	protected virtual void Awake()
	{
		Stats = GetComponent<Stats>();
		UnitAnim = Visual.GetComponent<UnitAnim>();
	}

	protected virtual void Start()
    {
		transform.position = Grid.GetClosestTile(transform.position).spawnPoint;
		CurrentActionPoints = StartActionPoints;
		UnitAnim.Idle(true);
	}

	public int CompareTo(UnitController obj)
	{
		return obj.Stats.PlayingTurnSpeed.CompareTo(Stats.PlayingTurnSpeed);
	}

	public virtual void StartRound()
    {
		print(name + " turn.");

		CurrentActionPoints = StartActionPoints + ReservedActionPoints;
        if (CurrentActionPoints > maxActionPoints)
        {
            CurrentActionPoints = maxActionPoints;
        }

		CalculatePossibleTiles();
		OnNewUnitTurn?.Invoke(this);

		MyTurn = true;
		state = State.ReadingInput;
    }

    public virtual void CalculatePossibleTiles(int _distance = 0)
    {
        int distance;

        if (_distance > 0)
        {
            distance = _distance;
        }
        else
        {
            distance = CurrentActionPoints;
        }

        ClearInfo();
       
        Tile currentTile = Grid.GetClosestTile(transform.position);
        Queue<Tile> tilesQueue = new Queue<Tile>();
        currentTile.SetStateContainsUnit();
        Grid.FindAllBlocked();

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
            nextTilePosition = movePath.Pop().spawnPoint;
            nextTiledirection = (nextTilePosition - transform.position).normalized;

			return true;
        }
        else
        {
            return false;
        }
    }

    protected void MakeStep(bool stopAfterOneTile = false)
    {
		UnitAnim.Idle(false);

		LookAt(nextTilePosition);
		transform.position += nextTiledirection * Time.deltaTime * Speed;

        if (Vector3.Distance(nextTilePosition, transform.position) <= 0.10f)
        {
            transform.position = nextTilePosition;
            WasteActionPoints(1, false);

			if (CalculateNextStep() == false)
			{
				CalculatePossibleTiles();
				state = State.ReadingInput;
				UnitAnim.Idle(true);
			}

			if (stopAfterOneTile == true)
			{
				state = State.ReadingInput;
			}
		}
    }

    public void LookAt(Transform target)
    {
		Visual.transform.LookAt(target);
    }

	public void LookAt(Vector3 target)
	{
		Visual.transform.LookAt(target);
	}

	public void Targeted()
    {
        GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
    }

    public void Untargeted()
    {
        GetComponentInChildren<Renderer>().material.DisableKeyword("_EMISSION");
    }

    public bool WasteActionPoints(int AP, bool RecalculatePossibleMoves = true)
    {
        if (CurrentActionPoints >= AP)
        {
            CurrentActionPoints -= AP;
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

	public void OpenInventory()
	{
		if (Inventory != null)
		{
			Inventory.Open();
		}
	}

	public void CloseInventory()
	{
		if (Inventory != null)
		{
			Inventory.Close();
		}
	}

	public virtual void WaitTurn()
    {

    }

    public virtual void EndTurn()
    {
		ReservedActionPoints = CurrentActionPoints;
        if (ReservedActionPoints > maxReservedActionPoints)
        {
            ReservedActionPoints = maxReservedActionPoints;
        }

		UnitAnim.Idle(true);
		DefocusTarget();
		ClearInfo();

		MyTurn = false;
		state = State.Waiting;
    }

    protected virtual void ClearInfo()
    {
        Grid.ResetAll();
        movePath.Clear();
    }

    public virtual void KillUnit()
    {
		if (weapon != null)
		{
			weapon.ItemVisual.GetComponentInChildren<Animated>().Disable();
			weapon.ItemVisual.transform.parent = null;
		}

		GetComponentInChildren<UnitAnim>().UnitDied();
		MyTurn = false;
    }
}