using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        [Tooltip("Relative chance of being picked. Higher = more common.")]
        public float weight = 1f;
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private EnemyType[] enemyTypes;

    [Header("Spawn Area")]
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 15f;
    [SerializeField] private float navMeshSampleDistance = 2f;
    [SerializeField] private float offscreenViewportPadding = 0.1f;

    [Header("Difficulty Ramp")]
    [Tooltip("How long (seconds) the initial easy phase lasts before ramping begins.")]
    [SerializeField] private float gracePeriod = 60f;

    [Tooltip("Spawn interval (seconds between spawns) during the grace period.")]
    [SerializeField] private float startSpawnInterval = 4f;

    [Tooltip("Spawn interval once fully ramped up (the floor it approaches).")]
    [SerializeField] private float minSpawnInterval = 0.75f;

    [Tooltip("How many seconds after the grace period it takes to reach minSpawnInterval.")]
    [SerializeField] private float rampDuration = 240f;

    [SerializeField] private int startMaxAliveEnemies = 6;
    [SerializeField] private int endMaxAliveEnemies = 30;

    private float nextSpawnTime = 0f;
    private float totalWeight = 0f;
    private float elapsedTime = 0f;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        foreach (var type in enemyTypes)
            totalWeight += type.weight;
    }

    private void Update()
    {
        if (player == null) return;

        elapsedTime += Time.deltaTime;

        if (Time.time < nextSpawnTime) return;

        float currentInterval = GetCurrentSpawnInterval();
        nextSpawnTime = Time.time + currentInterval;

        PruneDeadEnemies();

        int currentMax = GetCurrentMaxAliveEnemies();
        if (aliveEnemies.Count >= currentMax) return;

        TrySpawnEnemy();
    }

    private float GetCurrentSpawnInterval()
    {
        if (elapsedTime <= gracePeriod)
            return startSpawnInterval;

        float rampElapsed = elapsedTime - gracePeriod;
        float t = Mathf.Clamp01(rampElapsed / rampDuration);

        
        t = t * t * (3f - 2f * t);

        return Mathf.Lerp(startSpawnInterval, minSpawnInterval, t);
    }

    private int GetCurrentMaxAliveEnemies()
    {
        if (elapsedTime <= gracePeriod)
            return startMaxAliveEnemies;

        float rampElapsed = elapsedTime - gracePeriod;
        float t = Mathf.Clamp01(rampElapsed / rampDuration);
        t = t * t * (3f - 2f * t);

        return Mathf.RoundToInt(Mathf.Lerp(startMaxAliveEnemies, endMaxAliveEnemies, t));
    }

    private void TrySpawnEnemy()
    {
        GameObject prefabToSpawn = PickWeightedEnemy();
        if (prefabToSpawn == null) return;

        if (TryGetSpawnPoint(out Vector3 spawnPos))
        {
            GameObject instance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            aliveEnemies.Add(instance);
        }
    }

    private GameObject PickWeightedEnemy()
    {
        if (enemyTypes.Length == 0) return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var type in enemyTypes)
        {
            cumulative += type.weight;
            if (roll <= cumulative)
                return type.prefab;
        }

        return enemyTypes[enemyTypes.Length - 1].prefab;
    }

    private bool TryGetSpawnPoint(out Vector3 result)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized *
                                    Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector3 candidate = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (IsOnScreen(candidate))
                continue;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    private bool IsOnScreen(Vector3 worldPos)
    {
        if (mainCamera == null) return false;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPos);

        bool inFrontOfCamera = viewportPoint.z > 0f;
        bool withinBounds =
            viewportPoint.x > -offscreenViewportPadding &&
            viewportPoint.x < 1f + offscreenViewportPadding &&
            viewportPoint.y > -offscreenViewportPadding &&
            viewportPoint.y < 1f + offscreenViewportPadding;

        return inFrontOfCamera && withinBounds;
    }

    private void PruneDeadEnemies()
    {
        aliveEnemies.RemoveAll(e => e == null);
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        DrawCircle(player.position, minSpawnRadius);
        Gizmos.color = Color.red;
        DrawCircle(player.position, maxSpawnRadius);
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
