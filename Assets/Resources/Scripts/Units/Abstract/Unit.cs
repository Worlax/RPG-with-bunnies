using UnityEngine;
using System;
using System.Collections.Generic;

[SelectionBase]
public abstract class Unit: MonoBehaviour, IComparable<Unit>
{
	// Properties //
	// inspector
#pragma warning disable 0649

	[SerializeField] Transform _visual;
	[SerializeField] GameGrid _grid;
	[SerializeField] Inventory _inventoryPrefab;
	[SerializeField] Transform _windowsRoot;
	[SerializeField] int _money;

#pragma warning restore 0649

	[SerializeField] [Range(0.5f, 5f)] float _speed = 1.5f;
	[SerializeField] [Range(0, 10)] int _startActionPoints = 5;
	[ReadOnly] public Weapon weapon;
	[ReadOnly] public Unit unitInFocus;

	public Transform Visual { get => _visual; }
	public GameGrid Grid { get => _grid; }
	public Inventory InventoryPrefab { get => _inventoryPrefab; }
	public Transform WindowsRoot { get => _windowsRoot; }
	public int Money { get => _money; set => _money = value; }
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

	[SerializeField] [ReadOnly] bool _inBattle = false;
	public bool InBattle { get => _inBattle; private set => _inBattle = value; }
	public bool MyTurn { get; protected set; } = false;

	protected Stack<Tile> movePath = new Stack<Tile>();
	Vector3 nextTilePosition;
	Vector3 nextTiledirection;

	// Events //
	public static event Action<Unit> OnNewUnitTurn;
	public event Action OnInvolvedInBattle;
	public event Action OnLeavesTheBattle;

	public event Action OnMoneyChanged;

	// State //
	public enum BattleState
    {
        Waiting,
        ReadingInput,
        Moving
    }

	[ReadOnly] public BattleState battleState = BattleState.Waiting;

	// Functions //
	protected virtual void Awake()
	{
		Stats = GetComponent<Stats>();
		UnitAnim = Visual.GetComponent<UnitAnim>();
	}

	protected virtual void Start()
    {
		transform.position = Grid.GetClosestTile(transform.position).SpawnPoint;
		CurrentActionPoints = StartActionPoints;
		UnitAnim.Idle(true);
	}

	public int CompareTo(Unit obj)
	{
		return obj.Stats.PlayingTurnSpeed.CompareTo(Stats.PlayingTurnSpeed);
	}

	public virtual void StartBattle()
	{
		InBattle = true;
		Inventory.Close();
		Equipment.Close();
		battleState = BattleState.Waiting;

		OnInvolvedInBattle?.Invoke();
	}

	public virtual void EndBattle()
	{
		CurrentActionPoints = StartActionPoints;
		InBattle = false;
		OnLeavesTheBattle?.Invoke();
	}

	public virtual void StartRound()
    {
		CurrentActionPoints = StartActionPoints + ReservedActionPoints;
        if (CurrentActionPoints > maxActionPoints)
        {
            CurrentActionPoints = maxActionPoints;
        }

		CalculatePossibleTiles();
		OnNewUnitTurn?.Invoke(this);

		MyTurn = true;
		battleState = BattleState.ReadingInput;
    }

    public virtual void CalculatePossibleTiles(int _distance = 0, bool flagPossibleTiles = true)
    {
        int distance;

        if (_distance > 0)
        {
            distance = _distance;
        }
        else if (InBattle == false)
        {
            distance = 999;
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
					if (flagPossibleTiles)
					{
						tile.SetStatePossible();
					}
                    
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
            nextTilePosition = movePath.Pop().SpawnPoint;
            nextTiledirection = (nextTilePosition - transform.position).normalized;

			return true;
        }
        else
        {
            return false;
        }
    }

    protected void MakeStep(bool stopAfterOneTile = false, bool calculatePossibleTiles = true, BattleState stateAfterMoveEnd = BattleState.ReadingInput)
    {
		UnitAnim.Idle(false);

		LookAt(nextTilePosition);
		transform.position += nextTiledirection * Time.deltaTime * Speed;

        if (Vector3.Distance(nextTilePosition, transform.position) <= 0.10f)
        {
			if (InBattle)
			{
				WasteActionPoints(1, false);
			}

			transform.position = nextTilePosition;

			if (CalculateNextStep() == false)
			{
				UnitAnim.Idle(true);

				if (calculatePossibleTiles)
				{
					CalculatePossibleTiles();
				}
				
				battleState = stateAfterMoveEnd;
			}

			if (stopAfterOneTile == true)
			{
				battleState = stateAfterMoveEnd;
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

            return true;
        }
        else
        {
            return false;
        }
    }

	public virtual void FocusTarget(Unit target)
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

	public void AddMoney(int amount)
	{
		Money += amount;

		OnMoneyChanged?.Invoke();
	}

	public int TakeMoney(int amount)
	{
		int give = 0;

		if (Money >= amount)
		{
			give = amount;
			Money -= amount;
		}
		else
		{
			give = Money;
			Money = 0;
		}

		OnMoneyChanged?.Invoke();
		return give;
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
		battleState = BattleState.Waiting;
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