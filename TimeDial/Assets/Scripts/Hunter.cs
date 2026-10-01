using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Hunter : MonoBehaviour, IStunnable
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject skillOrbPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float maxHealth = 20.0f;

    [SerializeField] private float engageRange = 8f;    
    [SerializeField] private float preferredRange = 6f;  
    [SerializeField] private float retreatBuffer = 1f;   
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float updatePathRate = 0.2f;

    private NavMeshAgent agent;
    private float nextFireTime = 0f;
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

    private void Update()
    {
        agent.SetDestination(currentTarget.position);

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (Time.time >= nextPathUpdateTime)
        {
            UpdateMovement(distance);
            nextPathUpdateTime = Time.time + updatePathRate;
        }

        if (distance <= engageRange && HasLineOfSight())
        {
            FaceTarget();

            if (Time.time >= nextFireTime)
            {
                ShootAtPlayer();
                nextFireTime = Time.time + fireRate;
            }
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

    private void UpdateMovement(float distance)
    {
        if (distance > preferredRange)
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }
        else if (distance < preferredRange - retreatBuffer)
        {
            Vector3 fleeDir = (transform.position - currentTarget.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDir * (preferredRange - distance);
            agent.isStopped = false;
            agent.SetDestination(fleeTarget);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 targetPos = currentTarget.position + Vector3.up * 0.5f;
        Vector3 dir = targetPos - origin;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude))
        {
            return hit.transform == currentTarget || hit.collider.CompareTag("Player");
        }
        return true;
    }

    private void FaceTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Vector3 targetPos = currentTarget.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPos - spawnPos).normalized;

        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

        if (go.TryGetComponent(out EnemyProjectile projectile))
        {
            projectile.Launch(direction);
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
        if (roll <= chance && skillOrbPrefab != null)
        {
            Instantiate(skillOrbPrefab, transform.position, Quaternion.identity);
            Debug.Log("Zombie dropped a skill orb.");
        }
    }

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
        agent.isStopped = true;
    }
}
