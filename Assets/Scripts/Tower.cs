using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [Header("Car Prefab & Movement")]
    public GameObject carPrefab;    
    public float carSpeed = 5f; 

    [Header("Spawn Settings")]
    public float spawnInterval = 1f; 
    public float radius = 5f;

    private float timer;
    private MapManager mapMgr;
    private Grid grid;
    private List<Vector2Int> pathCells;

    void Start()
    {
        //Buscar referencias al mapa y al Grid
        mapMgr = FindObjectOfType<MapManager>();
        if (mapMgr == null)
            Debug.LogError("Tower: No se encontró MapManager en la escena.");
        else
            grid = mapMgr.grid;

        //Inicializar timer
        timer = spawnInterval;

        //Obtener la ruta generada
        if (mapMgr != null)
        {
            pathCells = mapMgr.PathCells;
            if (pathCells == null || pathCells.Count == 0)
                Debug.LogError("Tower: PathCells está vacío o no inicializado.");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = spawnInterval;
            //Comprobar si un enemigo en rango
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            foreach (var col in hits)
            {
                if (col.GetComponent<EnemyHealth>() != null)
                {
                    SpawnCar();
                    break;
                }
            }
        }
    }

    void SpawnCar()
    {
        if (pathCells == null || pathCells.Count == 0) return;

        Vector3 spawnPos = transform.position;
        GameObject car = Instantiate(carPrefab, spawnPos, Quaternion.identity);
        SoundManager.Instance?.PlayCarSpawn();
        var cm = car.GetComponent<CarMovement>();
        if (cm != null)
            cm.Init(pathCells, grid, carSpeed, spawnPos);
        else
            Debug.LogError("SpawnCar: carPrefab no tiene CarMovement.");
    }





    //Dibuja el radio de detección 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
