using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public EnemyData[] enemyTypes;
    public int poolSizePerType = 10;
    public Grid grid;
    public MapManager mapManager;

    [Header("Oleadas")]
    public int enemiesPerWave = 5;
    public float spawnInterval = 1f;

    [Header("UI")]
    public TMP_Text remainingText;

    private Dictionary<EnemyData, Queue<GameObject>> pools;
    private int spawned, kills;

    void Awake()
    {
        pools = new Dictionary<EnemyData, Queue<GameObject>>();
        foreach (var d in enemyTypes)
        {
            var q = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerType; i++)
            {
                var go = Instantiate(d.prefab);
                go.SetActive(false);
                q.Enqueue(go);
            }
            pools[d] = q;
        }
    }

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Espera hasta que MapManager haya generado la ruta
        yield return new WaitUntil(() =>
            mapManager.PathCells != null && mapManager.PathCells.Count > 0
        );
        // Ahora sí lanza la ola
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        while (spawned < enemiesPerWave)
        {
            SpawnEnemy(enemyTypes[spawned % enemyTypes.Length]);
            spawned++;
            UpdateRemainingUI();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnEnemy(EnemyData data)
    {
        var q = pools[data];
        if (q.Count == 0) return;
        var go = q.Dequeue();
        go.SetActive(true);
        go.GetComponent<EnemyMovement>()
          .Init(data, mapManager.PathCells, grid, this);
    }

    public void DespawnEnemy(GameObject e)
    {
        e.SetActive(false);
        foreach (var kv in pools)
            if (kv.Key.prefab.name == e.name.Replace("(Clone)", ""))
            { kv.Value.Enqueue(e); return; }
    }

    public void ReportKill()
    {
        kills++;
        UpdateRemainingUI();
        if (kills >= enemiesPerWave)
        {
            // Victoria
            var sc = FindObjectOfType<SceneController>();
            if (sc != null) sc.LoadVictoryScene();
            else Debug.LogError("SceneController no encontrado.");
        }
    }

    public void ReportReachedEnd()
    {
        // Derrota
        var sc = FindObjectOfType<SceneController>();
        if (sc != null) sc.LoadDefeatScene();
        else Debug.LogError("SceneController no encontrado para derrota.");
    }

    void UpdateRemainingUI()
    {
        if (remainingText != null)
            remainingText.text = $"Enemigos Restantes: {enemiesPerWave - kills}";
    }
}
