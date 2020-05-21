using UnityEngine;

public class GameGrid: MonoBehaviour
{
	// Properties //
#pragma warning disable 0649

	[SerializeField] Tile tilePrefab;

	[SerializeField] int columns = 10;
	[SerializeField] int rows = 10;
	[SerializeField] float gap = 0.8f;

#pragma warning restore 0649

	public Tile[,] Tiles { get; private set; }

    // Functions //
    void Awake()
    {
        Tiles = new Tile[columns, rows];

        for (int i = 0; i < columns; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                Tile tile = Instantiate(tilePrefab, new Vector3(transform.position.x + i * gap, transform.position.y, transform.position.z + j * gap), Quaternion.identity).GetComponent<Tile>();
                tile.transform.SetParent(transform);
                tile.name = "Tile [" + i + ", " + j + "]";

                Tiles[i, j] = tile;
            }
        }
    }

    public void FindAllBlocked()
    {
        foreach (Tile tile in Tiles)
        {
            tile.IsEmpty();
        }
    }

    public Tile GetClosestTile(Vector3 point, int range = 5)
    {
        Collider[] nearTiles = Physics.OverlapBox(point, Vector3.one * range, Quaternion.identity, LayerMask.GetMask("Tile"));
        Tile closestTile = Tiles[0, 0];
        float closestDistance = 999;

        foreach (Collider collider in nearTiles)
        {
            Tile tile = collider.GetComponent<Tile>();

            float distance = Vector3.Distance(point, tile.SpawnPoint);

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
        foreach (Tile tile in Tiles)
        {
            tile.Reset();
        }
    }
}
