using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapManager : MonoBehaviour
{

    [Header("Tamaño del Grid")]
    public int width = 10;
    public int height = 10;

    [Header("Start / End")]
    public Vector2Int startCell;
    public Vector2Int endCell;

    [Header("Prefabs")]
    public GameObject startPrefab;
    public GameObject endPrefab;
    public GameObject Calle;
    public GameObject BuildSopt; 
    public GameObject Banqueta;     
    public GameObject Edificios;

    [Header("Limite de Buldings Spots")]
    public int maxSpots = 6;

    [Header("Grid + Parent")]
    public Grid grid;
    public Transform mapParent;

    private bool[,] Ocupado;
    [SerializeField] private List<Vector2Int> pathCells;

    public List<Vector2Int> PathCells => pathCells;

    void Start()
    {
        GenerateMap();

    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        ClearMap();
        Ocupado = new bool[width, height];
        BuildPath();
        InstantiateTiles();
    }

    void ClearMap()
    {
        for (int i = mapParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(mapParent.GetChild(i).gameObject);
    }

    void BuildPath()
    {
        var dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var prev = new Dictionary<Vector2Int, Vector2Int>();
        var q = new Queue<Vector2Int>();
        prev[startCell] = startCell;
        q.Enqueue(startCell);

        while (q.Count > 0 && !prev.ContainsKey(endCell))
        {
            var cell = q.Dequeue();
            //Random de direcciones
            for (int i = 0; i < dirs.Length; i++)
            {
                int r = Random.Range(i, dirs.Length);
                (dirs[i], dirs[r]) = (dirs[r], dirs[i]);
            }
            foreach (var d in dirs)
            {
                var nxt = cell + d;
                if (nxt.x < 0 || nxt.x >= width || nxt.y < 0 || nxt.y >= height) continue;
                if (prev.ContainsKey(nxt)) continue;
                prev[nxt] = cell;
                q.Enqueue(nxt);
                if (nxt == endCell) break;
            }
        }

        pathCells = new List<Vector2Int>();
        if (!prev.ContainsKey(endCell))
        {
            Debug.LogError("No se encontro ruta");
            return;
        }
        var cur = endCell;
        while (true)
        {
            pathCells.Add(cur);
            if (cur == startCell) break;
            cur = prev[cur];
        }
        pathCells.Reverse();
    }

    void InstantiateTiles()
    {
        //Helper para instanciar y marcar ocupado
        void Place(Vector2Int cell, GameObject prefab)
        {
            Vector3 wp = grid.CellToWorld(new Vector3Int(cell.x, 0, cell.y));
            GameObject go = Instantiate(prefab, wp, Quaternion.identity, mapParent);
            Ocupado[cell.x, cell.y] = true;

            // Si es spot o sidewalk, orientar hacia carretera
            if (prefab == BuildSopt || prefab == Banqueta)
            {
                // Encuentra carretera más cercana
                Vector2Int nearest = pathCells
                    .OrderBy(c => Vector3.Distance(grid.CellToWorld(new Vector3Int(c.x, 0, c.y)), wp))
                    .First();
                Vector3 target = grid.CellToWorld(new Vector3Int(nearest.x, 0, nearest.y));
                go.transform.rotation = Quaternion.LookRotation((target - wp).normalized, Vector3.up);
            }
        }

        //Start / End
        Place(startCell, startPrefab);
        Place(endCell, endPrefab);

        //Camino
        foreach (var c in pathCells)
        {
            if (c == startCell || c == endCell) continue;
            Place(c, Calle);
        }

        //Recolectar celdas a un lado del camino
        var adjacent = new List<Vector2Int>();
        for (int i = 0; i < pathCells.Count; i++)
        {
            var c = pathCells[i];
            Vector2Int dir = (i < pathCells.Count - 1)
                ? pathCells[i + 1] - c
                : c - pathCells[i - 1];
            var left = new Vector2Int(-dir.y, dir.x);
            var right = new Vector2Int(dir.y, -dir.x);

            var lpos = c + left;
            var rpos = c + right;
            if (IsValid(lpos)) adjacent.Add(lpos);
            if (IsValid(rpos)) adjacent.Add(rpos);
        }
        var uniqueAdj = new List<Vector2Int>(new HashSet<Vector2Int>(adjacent));

        //maxSpots para buildingSpot
        var chosenSpots = new HashSet<Vector2Int>();
        var candidates = new List<Vector2Int>(uniqueAdj);
        while (chosenSpots.Count < maxSpots && candidates.Count > 0)
        {
            int idx = Random.Range(0, candidates.Count);
            chosenSpots.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }

        //Colocar buildingSpot y sidewalk en adyacentes
        foreach (var pos in uniqueAdj)
        {
            if (chosenSpots.Contains(pos))
                Place(pos, BuildSopt);
            else
                Place(pos, Banqueta);
        }

        //Rellenar el resto con edificios
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!Ocupado[x, y])
                {
                    Place(new Vector2Int(x, y), Edificios);
                }
            }
        }
    }

    bool IsValid(Vector2Int c)
    {
        return c.x >= 0 && c.x < width
            && c.y >= 0 && c.y < height
            && !Ocupado[c.x, c.y];
    }
}
