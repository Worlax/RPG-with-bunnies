using UnityEngine;

public class GameGrid: MonoBehaviour
{
    // Properties //
    public int columns = 10;
    public int rows = 10;
    public float gap = 0.8f;
    public Tile tileClassRef;

    public Tile[,] tiles;

    // Functions //
    void Awake()
    {
        tiles = new Tile[columns, rows];

        for (int i = 0; i < columns; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                Tile tile = Instantiate(tileClassRef, new Vector3(transform.position.x + i * gap, transform.position.y, transform.position.z + j * gap), Quaternion.identity).GetComponent<Tile>();
                tile.transform.SetParent(transform);
                tile.name = "Tile [" + i + ", " + j + "]";

                tiles[i, j] = tile;
            }
        }
    }

    public void FindAllBlocked()
    {
        foreach (Tile tile in tiles)
        {
            tile.IsEmpty();
        }
    }

    public Tile GetClosestTile(Vector3 point, int range = 5)
    {
        Collider[] nearTiles = Physics.OverlapBox(point, Vector3.one * range, Quaternion.identity, LayerMask.GetMask("Tile"));
        Tile closestTile = tiles[0, 0];
        float closestDistance = 999;

        foreach (Collider collider in nearTiles)
        {
            Tile tile = collider.GetComponent<Tile>();

            float distance = Vector3.Distance(point, tile.spawnPoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    public void ResetAll()
    {
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }
}
