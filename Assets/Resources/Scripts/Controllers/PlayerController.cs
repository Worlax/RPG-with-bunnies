using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class PlayerController: UnitController
{
	// Properties //
	[SerializeField] Equipment equipmentPrefab;

	[HideInInspector] public bool ignoreNextClick = false;

	Tile lastTileOverlaped;
	Tile lastTileClicked;

	Texture2D cursorOverTarget;

	float maxLootDistance = 1f;

	UnitController lootableTarget;
	Vector3 lootableInventoryPopupOffset = new Vector3(100, 0, 0);

	// states
	enum CursorState
	{
		Normal,
		OverTarget
	}

	CursorState cursorState = CursorState.Normal;

	// Events //
	public static event Action<UnitController> OnPlayerFocusedTarget;
    public static event Action OnPlayerDefocusedTarget;

	// Functions //
	protected override void Awake()
	{
		base.Awake();

		cursorOverTarget = Resources.Load<Texture2D>("Icons/Cursors/Selecting");
	}

	protected override void Start()
	{
		base.Start();

		Inventory = Instantiate(InventoryPrefab);
		Inventory.transform.SetParent(WindowsRoot.transform, false);
		Inventory.Owner = this;
		Inventory.name = "Inventory (" + transform.name + ")";
		
		Equipment = Instantiate(equipmentPrefab);
		Equipment.transform.SetParent(WindowsRoot.transform, false);
		Equipment.Owner = this;
		Equipment.name = "Equipment (" + transform.name + ")";
	}

    void Update()
    {
        if (state == State.Waiting)
        {
			return;
        }
        else if (CurrentActionPoints == 0)
        {
            EndTurn();
        }

        else if (state == State.ReadingInput)
        {
			if (IsMouseOverUI() == true)
			{
				if (lastTileOverlaped != null)
				{
					lastTileOverlaped.SetStatePossible();
					lastTileOverlaped = null;
				}

				return;
			}

			MouseOverlapCheck();

			if (Input.GetMouseButtonDown(0))
			{
				SelectingClick();
			}

			if (Input.GetMouseButtonDown(1))
            {
                if (ignoreNextClick)
                {
                    ignoreNextClick = false;
                    return;
                }

                MovementClick();
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

	// Reading input //
	List<RaycastResult> UIMouseRaycast()
	{
		EventSystem eventSystem = GameManager.instance.eventSystem;
		GraphicRaycaster graphicRaycaster = GameManager.instance.windowsGraphicRaycaster;

		List<RaycastResult> results = new List<RaycastResult>();
		PointerEventData pointerEventData = new PointerEventData(eventSystem);
		pointerEventData.position = Input.mousePosition;

		graphicRaycaster.Raycast(pointerEventData, results);

		return results;
	}

	bool IsMouseOverUI()
	{
		if (UIMouseRaycast().Count == 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

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

    void MovementClick()
    {
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

                CalculateAllSteps(lastTileClicked);
				state = State.Moving;
            }
        }
    }

    void SelectingClick()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.transform)
        {
            if (hit.transform.tag == "Enemy")
            {
                EnemyController enemy = hit.transform.GetComponentInParent<EnemyController>();
                
                // dead enemy
                if (hit.transform.GetComponentInParent<Stats>().Dead == true)
                {
                    TryToTootEnemy(hit.transform);
                }
                // second click on target
                else if (weapon != null && unitInFocus == enemy && weapon.AimedTarget != null)
                {
					if (WasteActionPoints(weapon.ActionPointsForUse) == true)
					{
						state = State.Waiting;
						weapon.Fire();
					}
                }
                // first click on target
                else
                {
                    if (weapon != null)
                    {
                        LookAt(hit.transform);
						UnitAnim.Idle(false);
                        weapon.Aim(enemy);
                    }
					
                    FocusTarget(enemy);
                }
            }
        }

		// click somewhere else
		if (unitInFocus != null)
		{
			if (hit.transform == null || hit.transform.tag != "Enemy")
			{
				DefocusTarget();

				if (weapon != null)
				{
					UnitAnim.Idle(true);
					weapon.StopAim();
				}
			}
		}
	}

	void TryToTootEnemy(Transform target)
    {
		if (lootableTarget != null && lootableTarget.transform == target.parent && lootableTarget.Inventory.Closed == false)
		{
			lootableTarget.CloseInventory();
			lootableTarget = null;
		}
		else
		{
			Vector3 direction = (target.position - transform.position).normalized;

			foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction, maxLootDistance))
			{
				if (hit.transform == target.transform)
				{
					lootableTarget = target.GetComponentInParent<UnitController>();
					lootableTarget.OpenInventory();
					lootableTarget.Inventory.transform.position = Input.mousePosition + lootableInventoryPopupOffset;

					Inventory.Open();

					return;
				}
			}
		}
    }

	public override void FocusTarget(UnitController target)
    {
		base.FocusTarget(target);

        OnPlayerFocusedTarget?.Invoke(target);
    }

    public override void DefocusTarget()
    {
		base.DefocusTarget();

        OnPlayerDefocusedTarget?.Invoke();
    }

    public override void WaitTurn()
    {
        base.WaitTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();

        SetCursorNormal();
    }

    protected override void ClearInfo()
    {
        base.ClearInfo();

		lastTileOverlaped = null;
        lastTileClicked = null;

		if (lootableTarget != null)
		{
			lootableTarget.CloseInventory();
			lootableTarget = null;
		}
    }

    void SomeUnitDied(Transform unit)
    {
        DefocusTarget();
    }
}