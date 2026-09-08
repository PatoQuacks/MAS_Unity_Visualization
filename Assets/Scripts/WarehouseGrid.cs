using UnityEngine;

public class WarehouseGrid : MonoBehaviour
{
    public Vector3 originTileCenter = new Vector3(0f, 0f, 0f); // center of tile (0,0), read from Inspector
    public float cellSize = 1f;
    public int columns = 20;
    public int rows = 20;

    public Vector3 CellToWorld(int col, int row)
    {
        float x = originTileCenter.x + col * cellSize;
        float z = originTileCenter.z + row * cellSize;
        return new Vector3(x, originTileCenter.y, z);
    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        int col = Mathf.RoundToInt((world.x - originTileCenter.x) / cellSize);
        int row = Mathf.RoundToInt((world.z - originTileCenter.z) / cellSize);
        return new Vector2Int(col, row);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Gizmos.DrawWireCube(CellToWorld(col, row), new Vector3(cellSize, 0.05f, cellSize));
            }
        }
    }
}