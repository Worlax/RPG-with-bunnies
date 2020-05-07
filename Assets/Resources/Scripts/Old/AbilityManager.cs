using UnityEngine;
using System.Collections.Generic;

public class AbilityManager: MonoBehaviour
{
    // Singleton //
    public static AbilityManager instance;

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
    }

    // Properties //
    Ability ability;
    PlayerController caster;
    List<string> PossibleTartgetTags = new List<string>();

    LineRenderer lineRenderer;

    UnitController overlappedTarget;

    // State //
    public enum State
    {
        Normal,
        Selecting
    }

    public State state = State.Normal;

    // Functions //
    void Update()
    {
        if (state == State.Selecting)
        {
            Selecting();

            if (Input.GetMouseButtonDown(0) && overlappedTarget != null)
            {
                Cast();
                ExitSelectingMode();
            }
            if (Input.GetMouseButtonDown(1))
            {
                caster.ignoreNextClick = true;
                ExitSelectingMode();
            }
        }
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Selecting mode //
    public void SetSkillInSelectingMode(Ability _skill, List<string> _tags)
    {
        ability = _skill;
        caster = GameManager.instance.currentUnit.GetComponent<PlayerController>();
        PossibleTartgetTags = _tags;

        if (caster == null)
        {
            ExitSelectingMode();
            return;
        }

        caster.state = PlayerController.State.Waiting;

        Texture2D cursorTexture = Resources.Load<Texture2D>("Cursors/selecting");
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width, cursorTexture.height) / 2, CursorMode.Auto);

        state = State.Selecting;
    }

    void Selecting()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitFromCamera);

        if (hitFromCamera.transform == null || CheckTagMatch(hitFromCamera.transform.tag) == false ||
            hitFromCamera.transform.GetComponent<Stats>().dead == true)
        {
            StopOldTargetOverlap();
            lineRenderer.positionCount = 0;
            return;
        }
        
        Physics.Raycast(caster.transform.position, (hitFromCamera.transform.position - caster.transform.position).normalized, out RaycastHit hitFromCaster);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, caster.transform.position);
        lineRenderer.SetPosition(1, hitFromCaster.point);

        OverlapNewTarget(hitFromCaster.transform);
    }

    bool CheckTagMatch(string _tag)
    {
        foreach (string tag in PossibleTartgetTags)
        {
            if (_tag == tag)
            {
                return true;
            }
        }

        return false;
    }

    void OverlapNewTarget(Transform newTarget)
    {
        if (newTarget == null)
            return;

        if (newTarget.GetComponent<UnitController>())
        {
            StopOldTargetOverlap();

            overlappedTarget = newTarget.GetComponent<UnitController>();
            overlappedTarget.Targeted();

            if (overlappedTarget.tag == "Enemy")
            {
                caster.FocusTarget(overlappedTarget.GetComponent<EnemyController>());
            }
        }
    }

    void StopOldTargetOverlap()
    {
        if (overlappedTarget != null)
        {
            overlappedTarget.Untargeted();
            overlappedTarget = null;
        }
    }

    // Cast and exit //
    void Cast()
    {
        if (overlappedTarget != null)
        {
            ability.Cast(caster, overlappedTarget, lineRenderer.GetPosition(1));
        }
    }

    void ExitSelectingMode()
    {
        ClearInfo();

        state = State.Normal;
    }

    //....//
    void ClearInfo()
    {
        ability = null;
        StopOldTargetOverlap();
        PossibleTartgetTags.Clear();
        lineRenderer.positionCount = 0;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (caster != null)
        {
            caster.state = PlayerController.State.ReadingInput;
        }
    }
}
