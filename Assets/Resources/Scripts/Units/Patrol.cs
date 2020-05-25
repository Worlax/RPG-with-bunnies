using UnityEngine;
using System.Collections.Generic;

public class Patrol: MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Transform flags;

#pragma warning restore 0649

	int stepCurrentIndex = -1;

	// Functions //
	public Stack<Tile> CalculateAllSteps()
	{
		GameGrid grid = GetComponent<Unit>().Grid;
		Stack<Tile> movePath = new Stack<Tile>();

		Tile tile = grid.GetClosestTile(flags.GetChild(GetNextIndex()).position);
		movePath.Push(tile);

		return movePath;
	}

	int GetNextIndex()
	{
		++stepCurrentIndex;
		int stepMaxIndex = flags.childCount - 1;

		if (stepCurrentIndex > stepMaxIndex)
		{
			stepCurrentIndex = 0;
		}

		return stepCurrentIndex;
	}
}
