using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Player: Unit
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Equipment equipmentPrefab;

#pragma warning restore 0649

	[HideInInspector] public bool ignoreNextClick = false;

	Tile lastTileOverlaped;
	Tile lastTileClicked;

	Texture2D cursorOverTarget;
	Texture2D cursorOverInteractable;

	float maxInteractDistance = 1f;

	Unit lootableTarget;
	Vector3 lootableInventoryPopupOffset = new Vector3(100, 0, 0);

	NPC interactedNPC;
	Vector3 NPCInventoryPopupOffset = new Vector3(-100, 0, 0);

	// Events //
	public static event Action<Unit> OnPlayerFocusedTarget;
    public static event Action OnPlayerDefocusedTarget;

	// Functions //
	protected override void Awake()
	{
		base.Awake();

		cursorOverTarget = Resources.Load<Texture2D>("Icons/Cursors/OverTarget");
		cursorOverInteractable = Resources.Load<Texture2D>("Icons/Cursors/OverInteractable");
	}

	protected override void Start()
	{
		base.Start();

		Inventory = Instantiate(InventoryPrefab);
		Inventory.transform.SetParent(WindowsRoot.transform, false);
		Inventory.Owner = this;
		Inventory.name = "Inventory (" + transform.name + ")";
		Inventory.Init();
		Inventory.Close();
		
		Equipment = Instantiate(equipmentPrefab);
		Equipment.transform.SetParent(WindowsRoot.transform, false);
		Equipment.Owner = this;
		Equipment.name = "Equipment (" + transform.name + ")";
		Equipment.Close();

		CalculatePossibleTiles();
	}

    void Update()
    {
		if (InBattle)
		{
			BattleLogic();
		}
		else
		{
			OutOfBattleLogic();
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

	public override void EndBattle()
	{
		base.EndBattle();

		CalculatePossibleTiles();
	}

	void OutOfBattleLogic()
	{
		if (battleState == BattleState.Moving)
		{
			MakeStep();
		}
		else
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
				SelectClick();
			}

			if (Input.GetMouseButtonDown(1))
			{
				if (ignoreNextClick)
				{
					ignoreNextClick = false;
					return;
				}

				MoveClick();
			}
		}
	}

	void BattleLogic()
	{
		if (battleState == BattleState.Waiting)
		{
			return;
		}
		else if (CurrentActionPoints == 0)
		{
			EndTurn();
		}

		else if (battleState == BattleState.ReadingInput)
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
				SelectClick();
			}

			if (Input.GetMouseButtonDown(1))
			{
				if (ignoreNextClick)
				{
					ignoreNextClick = false;
					return;
				}

				MoveClick();
			}
		}
		else if (battleState == BattleState.Moving)
		{
			MakeStep();
		}
	}

	List<RaycastResult> UIMouseRaycast()
	{
		EventSystem eventSystem = BattleManager.instance.eventSystem;
		GraphicRaycaster graphicRaycaster = BattleManager.instance.windowsGraphicRaycaster;

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
			SetCursor(null);
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

            // cursor texture
            if (hit.transform.tag == "Enemy")
            {
				SetCursor(cursorOverTarget);
			}
			else if (hit.transform.tag == "NPC")
			{
				SetCursor(cursorOverInteractable);
			}
            else
            {
				SetCursor(null);
			}

            // cursor over Tile
            if (hit.transform.tag == "Tile" && hit.transform.GetComponent<Tile>().state == Tile.State.Possible)
            {
                lastTileOverlaped = hit.transform.GetComponent<Tile>();
                lastTileOverlaped.SetStateOverlapped();
            }
        }
    }

	void SetCursor(Texture2D texture)
	{
		if (texture == null)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		else
		{
			Cursor.SetCursor(texture, new Vector2(cursorOverTarget.width, cursorOverTarget.height) / 2, CursorMode.Auto);
		}
		
	}

    void MoveClick()
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
				battleState = BattleState.Moving;
            }
        }
    }

    void SelectClick()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.transform)
        {
            if (hit.transform.tag == "Enemy")
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                
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
						if (InBattle == false)
						{
							BattleManager.instance.StartBattle(this, enemy);
							enemy.GetComponentInChildren<Aggro>().AddNearEnemysToBattle();
						}

						battleState = BattleState.Waiting;
						enemy.battleState = BattleState.Waiting;
						enemy.isPatroling = false;
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
			else if (hit.transform.tag == "NPC" && InBattle == false)
			{
				TryToInteractWithNPC(hit.transform);
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

			foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction, maxInteractDistance))
			{
				if (hit.transform == target.transform)
				{
					lootableTarget = target.GetComponentInParent<Unit>();
					lootableTarget.OpenInventory();
					lootableTarget.Inventory.transform.position = Input.mousePosition + lootableInventoryPopupOffset;

					Inventory.Open();

					return;
				}
			}
		}
    }

	void TryToInteractWithNPC(Transform target)
	{
		if (interactedNPC != null && interactedNPC.transform == target.transform.parent && interactedNPC.Inventory.Closed == false)
		{
			interactedNPC.CloseInventory();
			interactedNPC = null;
		}
		else
		{
			Vector3 direction = (target.position - transform.position).normalized;

			foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction, maxInteractDistance))
			{
				if (hit.transform == target)
				{
					interactedNPC = target.GetComponentInParent<NPC>();
					interactedNPC.OpenInventory();
					interactedNPC.Inventory.transform.position = Input.mousePosition + NPCInventoryPopupOffset;

					Inventory.Open();

					return;
				}
			}
		}
	}

	public override void FocusTarget(Unit target)
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

        SetCursor(null);
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