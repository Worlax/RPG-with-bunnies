using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager: MonoBehaviour
{
    // Singleton //
    public static GameManager instance = null;

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
    public List<UnitController> units = new List<UnitController>();
    public Queue<UnitController> unitsQueue = new Queue<UnitController>();
    public UnitController currentUnit;
    public bool playerMove = false;

    public LineRenderer WeaponAimLineRenderer;

    public Equipment equipment;

    public GraphicRaycaster windowsGraphicRaycaster;
    public EventSystem eventSystem;

	int roundN = 1;

	// Functions //
	void Update()
    {
		if (currentUnit != null)
		{
			if (currentUnit.MyTurn == false)
			{
				GiveNextUnitTurn();
			}
		}
	}

	void Start()
	{
		StartOfTheRound();
	}

    void StartOfTheRound()
    {
		// print("Round #" + roundN);
		++roundN;

        foreach (UnitController unit in FindObjectsOfType<UnitController>())
        {
            units.Add(unit);
        }

		units.Sort();
		foreach (UnitController unit in units)
		{
			unitsQueue.Enqueue(unit);
		}

		GiveNextUnitTurn();
	}

	void EndOfTheRound()
	{
		ClearInfo();
		StartOfTheRound();
	}

    void GiveNextUnitTurn()
    {
		if (unitsQueue.Count <= 0)
		{
			EndOfTheRound();
			return;
		}
		
        currentUnit = unitsQueue.Dequeue();

        if (currentUnit.GetComponent<Stats>().dead == true)
		{
			currentUnit = null;
			GiveNextUnitTurn();
			return;
		}

        if (currentUnit.tag == "Player")
        {
            playerMove = true;
        }
        else
        {
            playerMove = false;
        }

        currentUnit.StartRound();
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
		currentUnit.CalculatePossibleTiles();
	}

	void ClearInfo()
    {
        units.Clear();
        unitsQueue.Clear();
        currentUnit = null;
    }
}