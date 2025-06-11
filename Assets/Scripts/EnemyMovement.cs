using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    private float speed;
    private List<Vector3> worldWaypoints;
    private int index;
    private EnemyManager manager;

    public void Init(EnemyData data, List<Vector2Int> pathCells, Grid grid, EnemyManager mgr)
    {
        speed = data.speed;
        manager = mgr;
        worldWaypoints = new List<Vector3>(pathCells.Count);
        foreach (var cell in pathCells)
            worldWaypoints.Add(grid.CellToWorld(new Vector3Int(cell.x, 0, cell.y)));
        transform.position = worldWaypoints[0];
        index = 1;
    }

    void Update()
    {
        if (worldWaypoints == null || index >= worldWaypoints.Count)
        {
            manager.ReportReachedEnd();
            manager.DespawnEnemy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            worldWaypoints[index],
            speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, worldWaypoints[index]) < 0.01f)
            index++;

        if (index >= worldWaypoints.Count)
        {
            manager.ReportReachedEnd();
            manager.DespawnEnemy(gameObject);
            return;
        }
    }
}