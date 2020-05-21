using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIActionPoints: MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Color activePoints;
	[SerializeField] Color notActivePoints;

#pragma warning restore 0649

	PlayerController currentPlayer;

	public const int UIMaxActionPoints = 10;
    int UIActiveActionPoints;
    List<Image> actionPoints;

	// Functions //
	void Start()
    {
		currentPlayer = GameManager.instance.CurrenPlayer;

		actionPoints = new List<Image>();
		UIActiveActionPoints = UIMaxActionPoints;

		foreach (Transform child in transform)
        {
            Image ActionPointIMG = child.GetComponent<Image>();
            ActionPointIMG.color = activePoints;

            actionPoints.Add(ActionPointIMG);
        }
	}

	void Update()
	{
		ChangeActionPoints();
	}

    void OnEnable()
    {
        //UnitController.OnNewUnitTurn += ChangeActionPoints;
    }

    void OnDisable()
    {
        //UnitController.OnNewUnitTurn -= ChangeActionPoints;
    }

    void ChangeActionPoints()
    {
		int playerActionPoints = currentPlayer.CurrentActionPoints;

        if (UIActiveActionPoints > playerActionPoints)
        {
            UIActiveActionPoints = playerActionPoints;

            for (int i = UIMaxActionPoints - 1; i >= UIActiveActionPoints; --i)
            {
                actionPoints[i].color = notActivePoints;
            }
        }
        else if (UIActiveActionPoints < playerActionPoints)
        {
            UIActiveActionPoints = playerActionPoints;

            for (int i = 0; i < UIActiveActionPoints; ++i)
            {
                actionPoints[i].color = activePoints;
            }
        }
    }
}
