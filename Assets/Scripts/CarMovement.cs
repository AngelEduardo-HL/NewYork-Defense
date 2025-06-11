using UnityEngine;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    private List<Vector3> waypoints;
    private int index;
    private float speed;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    public void Init(List<Vector2Int> pathCells, Grid grid, float moveSpeed, Vector3 spawnPos)
    {
        speed = moveSpeed;

        // Convertir pathCells (start?end) a posiciones de mundo
        var world = new List<Vector3>(pathCells.Count);
        foreach (var cell in pathCells)
            world.Add(grid.CellToWorld(new Vector3Int(cell.x, 0, cell.y)));

        // Encontrar el waypoint mas cercano al spawn
        int nearest = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < world.Count; i++)
        {
            float d = Vector3.Distance(spawnPos, world[i]);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = i;
            }
        }

        // Construir waypoints
        waypoints = new List<Vector3>();
        for (int i = nearest; i >= 0; i--)
            waypoints.Add(world[i]);

        // Arrancar en 0
        index = 0;
    }

    void Update()
    {
        if (waypoints == null || index >= waypoints.Count)
        {
            SoundManager.Instance?.PlayCarDestroy();
            Destroy(gameObject);
            return;
        }

        // Mover hacia el waypoint actual
        Vector3 target = waypoints[index];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime);

        // Si llegamos al target, siguiente
        if (Vector3.Distance(transform.position, target) < 0.05f)
            index++;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rat"))
        {
            var health = other.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(1);
            SoundManager.Instance?.PlayCarDestroy();
            Destroy(gameObject);
        }
    }
}
