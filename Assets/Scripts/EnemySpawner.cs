using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Prefab do Inimigo (ex.: Orc.prefab)")]
    [SerializeField] GameObject enemyPrefab;

    [Header("Pontos de Spawn (opcional)")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] bool escolherPontoAleatorio = true;

    [Header("Respawn")]
    [SerializeField, Tooltip("Tempo entre morrer e nascer outro")]
    float respawnDelay = 1.5f;

    [SerializeField, Tooltip("Quantos inimigos devem existir no mínimo")]
    int minAlive = 1;

    [SerializeField, Tooltip("Limite máximo simultâneo (>= minAlive)")]
    int maxAlive = 1;

    int alive = 0;
    bool isSpawning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (spawnPoints == null || spawnPoints.Length == 0)
            spawnPoints = new Transform[] { this.transform };

        if (maxAlive < minAlive) maxAlive = minAlive;
    }

    void Start()
    {
        var existentes = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        alive = existentes.Length;

        if (alive < minAlive)
            StartCoroutine(SpawnUntilMinAlive());
    }

    IEnumerator SpawnUntilMinAlive()
    {
        if (isSpawning) yield break;
        isSpawning = true;

        while (alive < minAlive)
        {
            SpawnOne();
            yield return new WaitForEndOfFrame();
        }

        isSpawning = false;
    }

    // === CHAMADO PELO ENEMY AO NASCER ===
    public void NotifySpawned()
    {
        alive++;
    }

    // === CHAMADO PELO ENEMY AO MORRER ===
    public void NotifyDeath()
    {
        alive = Mathf.Max(0, alive - 1);

        if (alive < minAlive)
            StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        while (alive < Mathf.Min(minAlive, maxAlive))
        {
            SpawnOne();
            yield return null;
        }
    }

    public void SpawnOne()
    {
        if (!enemyPrefab)
        {
            Debug.LogWarning("[EnemySpawner] Falta atribuir o 'enemyPrefab' no Inspector.");
            return;
        }

        if (alive >= maxAlive) return;

        Transform p = escolherPontoAleatorio
            ? spawnPoints[Random.Range(0, spawnPoints.Length)]
            : spawnPoints[0];

        var go = Instantiate(enemyPrefab, p.position, Quaternion.identity);

        var enemy = go.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning("[EnemySpawner] O prefab não tem componente 'Enemy'.");
            alive++;
        }
    }
}
