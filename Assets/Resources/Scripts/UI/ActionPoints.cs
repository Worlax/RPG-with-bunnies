using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActionPoints: MonoBehaviour
{
    //============//
    // Properties //
    //============//

    public int UIMaxActionPoints = 10;
    int UIActiveActionPoints;
    List<Image> actionPoints = new List<Image>();
    public Color pointActive;
    public Color pointNotActive;

    //===========//
    // Functions //
    //===========//

    void Start()
    {
        foreach (Transform child in transform)
        {
            Image ActionPointIMG = child.GetComponent<Image>();
            ActionPointIMG.color = pointActive;

            actionPoints.Add(ActionPointIMG);
        }

        UIActiveActionPoints = UIMaxActionPoints;
    }

    void OnEnable()
    {
        UnitController.OnActionPointsChanged += ChangeActionPoints;
        UnitController.OnNewUnitTurn += ChangeActionPoints;
    }

    void OnDisable()
    {
        UnitController.OnActionPointsChanged -= ChangeActionPoints;
        UnitController.OnNewUnitTurn -= ChangeActionPoints;
    }

    void ChangeActionPoints(UnitController unit)
    {
        if (unit.tag != "Player")
            return;

        int playerActionPoints = GameManager.instance.currentUnit.currentActionPoints;

        if (UIActiveActionPoints > playerActionPoints)
        {
            UIActiveActionPoints = playerActionPoints;

            for (int i = UIMaxActionPoints - 1; i >= UIActiveActionPoints; --i)
            {
                actionPoints[i].color = pointNotActive;
            }
        }
        else if (UIActiveActionPoints < playerActionPoints)
        {
            UIActiveActionPoints = playerActionPoints;

            for (int i = 0; i < UIActiveActionPoints; ++i)
            {
                actionPoints[i].color = pointActive;
            }
        }
    }
}
