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

	Unit currentUnit;

	public const int UIMaxActionPoints = 10;
    int UIActiveActionPoints;
    List<Image> actionPoints;

	// Functions //
	void Start()
    {
		actionPoints = new List<Image>();
		UIActiveActionPoints = UIMaxActionPoints;

		foreach (Transform child in transform)
        {
            Image ActionPointIMG = child.GetComponent<Image>();
            ActionPointIMG.color = activePoints;

            actionPoints.Add(ActionPointIMG);
        }

		ConnectUnit(GameManager.instance.CurrenPlayer);
	}

	void Update()
	{
		ChangeActionPoints();
	}

	void ConnectUnit(Unit unit)
	{
		currentUnit = unit;

		currentUnit.OnInvolvedInBattle += Show;
		currentUnit.OnLeavesTheBattle += Hide;
	}

	void DisconnectUnit()
	{
		currentUnit.OnInvolvedInBattle -= Show;
		currentUnit.OnLeavesTheBattle -= Hide;

		currentUnit = null;
	}

    void ChangeActionPoints()
    {
		int playerActionPoints = currentUnit.CurrentActionPoints;

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

	void Show()
	{
		foreach (Image image in actionPoints)
		{
			image.transform.localScale = new Vector3(1, 1, 1);
		}
	}

	void Hide()
	{
		foreach (Image image in actionPoints)
		{
			image.transform.localScale = new Vector3(0, 0, 0);
		}
	}
}
