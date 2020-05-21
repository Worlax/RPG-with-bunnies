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

	public List<UnitController> BattlingUnits { get; private set; }
	public Queue<UnitController> UnitsTurnQueue { get; private set; }
	public UnitController CurrentUnit { get; private set; }

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
		BattlingUnits = new List<UnitController>();
		UnitsTurnQueue = new Queue<UnitController>();
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

	public void Provoke(UnitController provoker, UnitController target)
	{
		StartBattle(provoker, target);
	}

	public void StartBattle(params UnitController[] _units)
	{
		BattlingUnits = new List<UnitController>(_units);

		foreach (UnitController unit in BattlingUnits)
		{
			unit.StartBattle();
		}

		roundN = 0;
		StartNewRound();

		battleActive = true;
	}

	public void EndBattle()
	{
		foreach (UnitController unit in BattlingUnits)
		{
			unit.EndBattle();
		}

		battleActive = false;
		ClearInfo();
	}

	public void AddUnitToBattle(UnitController _unit)
	{
		_unit.StartBattle();
		BattlingUnits.Add(_unit);

		List<UnitController> reserve = UnitsTurnQueue.ToList<UnitController>();
		UnitsTurnQueue.Clear();
		reserve.Add(_unit);
		reserve.Sort();

		foreach (UnitController unit in reserve)
		{
			UnitsTurnQueue.Enqueue(unit);
		}
	}

    void StartNewRound()
    {
		++roundN;

		BattlingUnits.Sort();
		foreach (UnitController unit in BattlingUnits)
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
		foreach (UnitController unit in BattlingUnits)
		{
			if (unit is EnemyController && unit.GetComponent<Stats>().Dead == false)
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