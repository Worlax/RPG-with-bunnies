using UnityEngine;
using System.Collections.Generic;

[SelectionBase]
public class Tile: MonoBehaviour
{
    // Properties // test 3 4
    public Material mDisabled;
    public Material mPossible;
    public Material mOverlapped;
    public Material mClicked;
    public Material mWithPlayer;

    public Vector3 spawnPoint;

    public bool bChecked = false;
    public Tile parent;
    public int numberOfParents = 0;

    List<Tile> adjacentTiles = new List<Tile>();

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

    public State state = State.Disabled;

    // Functions //
    void Awake()
    {
        spawnPoint = transform.GetChild(0).transform.position;
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
        LayerMask mask = LayerMask.GetMask("Tile") | LayerMask.GetMask("Ground") | LayerMask.GetMask("Ignore Tile Raycast");
        Vector3 size = GetComponent<BoxCollider>().bounds.size / 2;

        if (Physics.CheckBox(transform.position, size, Quaternion.identity, ~mask))
        {
            state = State.Blocked;
            return true;
        }

        return false; // test 3
    }

    // Set state //
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

    //....//
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
