using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

public class BattleManager: MonoBehaviour
{
    // Singleton //
    public static BattleManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

		WeaponAimLineRenderer = GetComponent<LineRenderer>();
    }

	// Properties //
	public bool battleActive { get; private set; } = false;
	public int roundN { get; private set; }

	public List<Unit> BattlingUnits { get; private set; }
	public Queue<Unit> UnitsTurnQueue { get; private set; }
	public Unit CurrentUnit { get; private set; }

	public bool PlayerMove { get; private set; } = false;
	
	public LineRenderer WeaponAimLineRenderer { get; private set; }

	// todo move it somewhere else
	public GraphicRaycaster windowsGraphicRaycaster;
    public EventSystem eventSystem;

	// Functions //
	void Update()
    {
		if (battleActive)
		{
			if (CurrentUnit != null)
			{
				if (CurrentUnit.MyTurn == false)
				{
					GiveNextUnitTurn();
				}
			}
			else
			{
				GiveNextUnitTurn();
			}
		}
	}

	void Start()
	{
		BattlingUnits = new List<Unit>();
		UnitsTurnQueue = new Queue<Unit>();
	}

	void OnEnable()
	{
		Stats.OnUnitDied += UnitDied;
	}

	void OnDisable()
	{
		Stats.OnUnitDied -= UnitDied;
	}

	void UnitDied(Transform unit)
	{
		if (IsThereALivingEnemy() == true)
		{
			CurrentUnit.CalculatePossibleTiles();
		}
		else
		{
			EndBattle();
		}
	}

	public void Provoke(Unit provoker, Unit target)
	{
		StartBattle(provoker, target);
	}

	public void StartBattle(params Unit[] _units)
	{
		BattlingUnits = new List<Unit>(_units);

		foreach (Unit unit in BattlingUnits)
		{
			unit.StartBattle();
		}

		roundN = 0;
		StartNewRound();

		battleActive = true;
	}

	public void EndBattle()
	{
		PlayerMove = false;

		foreach (Unit unit in BattlingUnits)
		{
			unit.EndBattle();
		}

		ClearInfo();
		battleActive = false;
	}

	public void AddUnitToBattle(Unit _unit)
	{
		_unit.StartBattle();
		BattlingUnits.Add(_unit);

		List<Unit> reserve = UnitsTurnQueue.ToList<Unit>();
		UnitsTurnQueue.Clear();
		reserve.Add(_unit);
		reserve.Sort();

		foreach (Unit unit in reserve)
		{
			UnitsTurnQueue.Enqueue(unit);
		}
	}

    void StartNewRound()
    {
		++roundN;

		BattlingUnits.Sort();
		foreach (Unit unit in BattlingUnits)
		{
			UnitsTurnQueue.Enqueue(unit);
		}
	}

    void GiveNextUnitTurn()
    {
		CurrentUnit = null;

		if (UnitsTurnQueue.Count == 0)
		{
			StartNewRound();
		}
		else
		{
			CurrentUnit = UnitsTurnQueue.Dequeue();

			if (CurrentUnit.GetComponent<Stats>().Dead == true)
			{
				CurrentUnit = null;
				GiveNextUnitTurn();
				return;
			}
			else
			{
				if (CurrentUnit.tag == "Player")
				{
					PlayerMove = true;
				}
				else
				{
					PlayerMove = false;
				}

				CurrentUnit.StartRound();
			}
		}
    }

	bool IsThereALivingEnemy()
	{
		foreach (Unit unit in BattlingUnits)
		{
			if (unit is Enemy && unit.GetComponent<Stats>().Dead == false)
			{
				return true;
			}
		}

		return false;
	}

	void ClearInfo()
    {
        BattlingUnits.Clear();
        UnitsTurnQueue.Clear();
        CurrentUnit = null;
		PlayerMove = false;
	}
}