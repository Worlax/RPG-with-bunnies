using UnityEngine;
using System.Collections.Generic;

[SelectionBase]
public class Tile: MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Transform _spawnPoint;

	[SerializeField] Material mDisabled;
	[SerializeField] Material mPossible;
	[SerializeField] Material mOverlapped;
	[SerializeField] Material mClicked;
	[SerializeField] Material mWithPlayer;

#pragma warning restore 0649

	public Vector3 SpawnPoint { get => _spawnPoint.position; }

	[HideInInspector] public bool bChecked = false;
	[HideInInspector] public Tile parent;
	[HideInInspector] public int numberOfParents = 0;
    List<Tile> adjacentTiles;

    // State //
    public enum State
    {
        Disabled,
        Blocked,
        Possible,
        Overlapped,
        Clicked,
        ContainsUnit
    }

    [ReadOnly] public State state = State.Disabled;

    // Functions //
    void Start()
    {
		adjacentTiles = new List<Tile>();
	}

    public void FindAdjacentTiles()
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, Vector3.forward, out hit, 5);
        if (hit.transform != null && hit.transform.tag == "Tile")
        {
            adjacentTiles.Add(hit.transform.GetComponent<Tile>());
        }

        Physics.Raycast(transform.position, -Vector3.forward, out hit, 5);
        if (hit.transform != null && hit.transform.tag == "Tile")
        {
            adjacentTiles.Add(hit.transform.GetComponent<Tile>());
        }

        Physics.Raycast(transform.position, Vector3.right, out hit, 5);
        if (hit.transform != null && hit.transform.tag == "Tile")
        {
            adjacentTiles.Add(hit.transform.GetComponent<Tile>());
        }

        Physics.Raycast(transform.position, -Vector3.right, out hit, 5);
        if (hit.transform != null && hit.transform.tag == "Tile")
        {
            adjacentTiles.Add(hit.transform.GetComponent<Tile>());
        }
    }

    public List<Tile> GetAdjacentTiles()
    {
        FindAdjacentTiles();
        return adjacentTiles;
    }

    public bool IsEmpty()
    {
        LayerMask mask = LayerMask.GetMask("Tile") | LayerMask.GetMask("Ground") | LayerMask.GetMask("Item");
        Vector3 size = GetComponent<BoxCollider>().bounds.size / 2;

        if (Physics.CheckBox(transform.position, size, Quaternion.identity, ~mask))
        {
            state = State.Blocked;
            return true;
        }

        return false;
    }

    public void SetStateDisabled()
    {
        state = State.Disabled;
        SetMaterial(mDisabled);
    }

    public void SetStateBlocked()
    {
        state = State.Blocked;
        SetMaterial(mDisabled);
    }

    public void SetStatePossible()
    {
        state = State.Possible;
        SetMaterial(mPossible);
    }

    public Tile SetStateOverlapped()
    {
        state = State.Overlapped;
        SetMaterial(mOverlapped);

        return this;
    }

    public void SetStateClicked()
    {
        state = State.Clicked;
        SetMaterial(mClicked);
    }

    public void SetStateContainsUnit()
    {
        state = State.ContainsUnit;
        SetMaterial(mWithPlayer);
    }

    void SetMaterial(Material mat)
    {
        transform.GetComponent<Renderer>().material = mat;
    }

    public void Reset()
    {
        bChecked = false;
        parent = null;
        numberOfParents = 0;
        SetStateDisabled();
    }
}
