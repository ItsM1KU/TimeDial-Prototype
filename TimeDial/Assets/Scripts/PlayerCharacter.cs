using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private RectTransform crosshair;

    [SerializeField] private float projectileSpawnDistance = 0.75f;

    [SerializeField] private float fireRate = 0.5f; 
    private float nextFireTime = 0f;

    [SerializeField] private AimController aimController;

    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            ShootProjectile(aimController.AimDirectionWorld);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition = transform.position + direction * projectileSpawnDistance;
        GameObject go = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        if (go.TryGetComponent(out PlayerProjectile projectile))
            projectile.Launch(direction);
    }
}
