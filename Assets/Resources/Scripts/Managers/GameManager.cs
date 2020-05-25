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

		CurrenPlayer = FindObjectOfType<Player>();
	}

	// Properties //
	public Player CurrenPlayer { get; private set; }

	// Functions //
}
