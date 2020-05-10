using UnityEngine;
using System;

public class PlayerController: UnitController
{
    // Properties //
    Tile lastTileOverlaped;
    Tile lastTileClicked;

    public EnemyController enemyInFocus;

    public bool ignoreNextClick = false;

    Texture2D cursorOverTarget;

    enum CursorState
    {
        Normal,
        OverTarget
    }

    CursorState cursorState = CursorState.Normal;

    // Events //
    public static event Action<UnitController> OnFocusTarget;
    public static event Action OnDefocusTarget;

    // Functions //
    protected override void Update()
    {
        base.Update();

        if (state == State.NotMyMove)
        {
            return;
        }

        if (currentActionPoints == 0)
        {
            EndTurn();
        }

        if (state == State.RoundStart)
        {
            CalculatePossibleMoves();
            state = State.ReadingInput;
        }
        else if (state == State.ReadingInput)
        {
            MouseOverlapCheck();

            if (Input.GetMouseButtonDown(1))
            {
                if (ignoreNextClick)
                {
                    ignoreNextClick = false;
                    return;
                }

                TileCkick();
            }
            if (Input.GetMouseButtonDown(0))
            {
                TargetClick();
            }
        }
        else if (state == State.CalculatingStep)
        {
            if (CalculateStep())
            {
                state = State.Moving;
            }
            else if (currentActionPoints > 0)
            {
                state = State.RoundStart;
            }
        }
        else if (state == State.Moving)
        {
            MakeStep();
        }
    }

    void OnEnable()
    {
        Stats.OnUnitDied += SomeUnitDied;
    }

    void OnDisable()
    {
        Stats.OnUnitDied -= SomeUnitDied;
    }

    protected override void Start()
    {
        base.Start();

        cursorOverTarget = Resources.Load<Texture2D>("Icons/Cursors/Selecting");
    }

    // Reading input //
    void MouseOverlapCheck()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.transform)
        {
            if (lastTileOverlaped != null && lastTileOverlaped.transform == hit.transform)
            {
                return;
            }

            if (lastTileOverlaped != null)
            {
                lastTileOverlaped.SetStatePossible();
                lastTileOverlaped = null;
            }

            // cursor over Enemy
            if (hit.transform.tag == "Enemy" && cursorState != CursorState.OverTarget)
            {
                SetCursorOverTarget(hit.transform.GetComponent<UnitController>());
            }
            else if (hit.transform.tag != "Enemy" && cursorState != CursorState.Normal)
            {
                SetCursorNormal();
            }

            // cursor over Tile
            if (hit.transform.tag == "Tile" && hit.transform.GetComponent<Tile>().state == Tile.State.Possible)
            {
                lastTileOverlaped = hit.transform.GetComponent<Tile>();
                lastTileOverlaped.SetStateOverlapped();
            }
        }
    }

    void SetCursorOverTarget(UnitController unit)
    {
        Cursor.SetCursor(cursorOverTarget, new Vector2(cursorOverTarget.width, cursorOverTarget.height) / 2, CursorMode.Auto);

        cursorState = CursorState.OverTarget;
    }

    void SetCursorNormal()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        cursorState = CursorState.Normal;
    }

    void TileCkick()
    {
        //if (weapon != null && weapon.fireAnimationInProcess == true)
        //    return;

        if (lastTileClicked != null)
        {
            lastTileClicked.SetStatePossible();
            lastTileClicked = null;
        }

        if (weapon != null)
        {
            weapon.StopAim();
        }

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.transform)
        {
            if (hit.transform.tag == "Tile" && hit.transform.GetComponent<Tile>().state == Tile.State.Overlapped)
            {
                lastTileClicked = hit.transform.GetComponent<Tile>();
                lastTileClicked.SetStateClicked();

                CalculatePath(lastTileClicked);
                state = State.CalculatingStep;
            }
        }
    }

    void TargetClick()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.transform)
        {
            if (hit.transform.tag == "Enemy")
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                
                // dead enemy
                if (hit.transform.GetComponent<Stats>().dead == true)
                {
                    TryToToot(hit.transform);
                }
                // second click on target
                else if (weapon != null && enemyInFocus == enemy && weapon.AimedTarget != null)
                {
                    weapon.Fire();
                }
                // first click on target
                else
                {
                    if (weapon != null)
                    {
                        LookAtTarget(hit.transform);
                        weapon.Aim(enemy);
                    }

                    FocusTarget(enemy);
                }
            }
        }

        // click somewhere else
        //if (enemyInFocus != null)
        //{
        //    if (hit.transform == null || hit.transform.tag != "Enemy")
        //    {
        //        DefocusTarget();

        //        if (weapon != null)
        //        {
        //            weapon.StopAim();
        //        }
        //    }
        //}
    }

    void TryToToot(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float maxLootDistance = 30;

        foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction, maxLootDistance))
        {
            if (hit.transform == target.transform)
            {
                Lootable loot = target.GetComponent<Lootable>();
                loot.Open(transform);

                return;
            }
        } 
    }

    void LookAtTarget(Transform target)
    {
        transform.LookAt(target);
    }

    // Focus target //
    public void FocusTarget(EnemyController target)
    {
        enemyInFocus = target;

        OnFocusTarget?.Invoke(target);
    }

    public void DefocusTarget()
    {
        enemyInFocus = null;

        if (weapon != null)
        {
            weapon.StopAim();
        }

        OnDefocusTarget?.Invoke();
    }

    //....//
    public override void WaitTurn()
    {
        base.WaitTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();

        DefocusTarget();
        SetCursorNormal();
        ClearInfo();
    }

    protected override void ClearInfo()
    {
        base.ClearInfo();
        lastTileOverlaped = null;
        lastTileClicked = null;
    }

    void SomeUnitDied(Transform unit)
    {
        DefocusTarget();
    }
}