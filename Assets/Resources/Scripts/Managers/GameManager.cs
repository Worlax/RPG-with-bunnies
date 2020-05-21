using UnityEngine;
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

		CurrenPlayer = FindObjectOfType<PlayerController>();
	}

	// Properties //
	public PlayerController CurrenPlayer { get; private set; }

	// Functions //
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			BattleManager.instance.StartBattle(FindObjectsOfType<UnitController>());
		}
	}
}
