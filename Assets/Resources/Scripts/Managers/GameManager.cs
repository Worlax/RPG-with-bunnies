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

    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;

    float lastTimeClicked = 0;
    float doubleClickMaxSpread = 0.25f;

    // State //
    public enum State
    {
        StartOfTheRound,
        GiveNextPlayerTurn,
        WaitingForMove,
        EndOfTheRound
    }

    public State state = State.StartOfTheRound;

    // Functions //
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Click();
        }

        if (state == State.StartOfTheRound)
        {
            StartOfTheRound();
            state = State.GiveNextPlayerTurn;
        }
        else if (state == State.GiveNextPlayerTurn)
        {
            
            if (unitsQueue.Count > 0)
            {
                if (GiveUnitATurn() == true)
                {
                    state = State.WaitingForMove;
                }
            }
            else
            {
                state = State.EndOfTheRound;
            }
        }
        else if (state == State.WaitingForMove)
        {
            if (currentUnit.state == UnitController.State.NotMyMove)
            {
                state = State.GiveNextPlayerTurn;
            }

        }
        else if (state == State.EndOfTheRound)
        {
            state = State.StartOfTheRound;
        }
    }

    void Click()
    {
        if (lastTimeClicked == 0)
        {
            lastTimeClicked = Time.time;
            return;
        }

        if (Time.time - lastTimeClicked < doubleClickMaxSpread)
        {
            List<RaycastResult> UIClickResult = UIMouseRaycast();
            
            foreach (RaycastResult obj in UIClickResult)
            {
                Usable consumable = obj.gameObject.GetComponent<Usable>();

                if (consumable != null)
                {
                    consumable.Use();
                    break;
                }
            }

            if (UIClickResult.Count == 0)
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

                if (hit.transform != null)
                {

                }
            }
        }

        lastTimeClicked = Time.time;
    }

    List<RaycastResult> UIMouseRaycast()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        graphicRaycaster.Raycast(pointerEventData, results);

        return results;
    }

    void OnEnable()
    {
        PlayerController.OnUnitEndTurn += GiveNextPlayerTurn;
        Stats.OnUnitDied += UnitDied;
    }

    void OnDisable()
    {
        PlayerController.OnUnitEndTurn -= GiveNextPlayerTurn;
        Stats.OnUnitDied -= UnitDied;
    }

    void GiveNextPlayerTurn(UnitController unit)
    {
        state = State.GiveNextPlayerTurn;
    }

    void UnitDied(Transform unit)
    {
        currentUnit.CalculatePossibleMoves();
    }

    void StartOfTheRound()
    {
        ClearInfo();

        foreach (UnitController unit in FindObjectsOfType<UnitController>())
        {
            if (unit.GetComponent<Stats>().dead == false)
            {
                units.Add(unit);
                unitsQueue.Enqueue(unit);
            }
        }

        //print(units.Count + " " + unitsQueue.Count);
    }

    bool GiveUnitATurn()
    {
        currentUnit = unitsQueue.Dequeue();

        if (currentUnit.GetComponent<Stats>().dead == true)
            return false;

        if (currentUnit.tag == "Player")
        {
            playerMove = true;
        }
        else
        {
            playerMove = false;
        }

        currentUnit.StartRound();

        return true;
    }

    //....//

    void ClearInfo()
    {
        units.Clear();
        unitsQueue.Clear();
        currentUnit = null;
    }
}