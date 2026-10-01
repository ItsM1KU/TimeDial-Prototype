using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour, IStunnable
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxHealth = 20.0f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 1f; 
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float updatePathRate = 0.2f; 

    [SerializeField] private GameObject healthOrbPrefab;

    private NavMeshAgent agent;
    private float nextDamageTime = 0f;
    private float nextPathUpdateTime = 0f;
    private float currentHealth;

    private bool isStunned = false;
    private float stunEndTime = 0f;

    private Transform currentTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        currentTarget = player;
    }

    private void Update()
    {

        if (Time.time >= nextPathUpdateTime)
        {
            agent.SetDestination(currentTarget.position);
            nextPathUpdateTime = Time.time + updatePathRate;
        }

        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;
                agent.isStopped = false;
            }
            else
            {
                return;
            }
        }
    }

    public void SetTarget(Transform newTarget) => currentTarget = newTarget;
    public void ClearTarget()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        currentTarget = player;
    }

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
        agent.isStopped = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider col)
    {
        if (!col.CompareTag("Player")) return;
        if (Time.time < nextDamageTime) return;

        if (col.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Zombie took {amount} damage, current health: {currentHealth}");
        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        // handle death
        Debug.Log("Zombie has died.");
        TrySpawnHealthOrb();
        Destroy(gameObject);
    }

    private void TrySpawnHealthOrb()
    {
        float chance = 0.1f; // 10% chance to drop a health orb
        float roll = Random.Range(0f, 1f);
        if (roll <= chance && healthOrbPrefab != null)
        {
            Instantiate(healthOrbPrefab, transform.position, Quaternion.identity);
            Debug.Log("Zombie dropped a health orb.");
        }
    }
}
