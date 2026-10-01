using UnityEngine;

public class StunOrbProjectile : MonoBehaviour
{
    [SerializeField] private float travelSpeed = 15f;
    [SerializeField] private LayerMask hitLayers; 

    private Vector3 direction;
    private float maxDistance;
    private float stunRadius;
    private float stunDuration;
    private Vector3 startPos;
    private bool detonated = false;

    [SerializeField] private GameObject stunDomeVisualPrefab;

    public void Init(Vector3 dir, float maxDist, float radius, float duration)
    {
        direction = dir.normalized;
        maxDistance = maxDist;
        stunRadius = radius;
        stunDuration = duration;
        startPos = transform.position;
    }

    private void Update()
    {
        if (detonated) return;

        transform.position += direction * travelSpeed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            Detonate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (detonated) return;

        if (other.CompareTag("Enemy"))
            Detonate();
    }

    private void Detonate()
    {
        detonated = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, stunRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IStunnable stunnable))
                stunnable.ApplyStun(stunDuration);
        }

        if (stunDomeVisualPrefab != null)
        {
            GameObject dome = Instantiate(stunDomeVisualPrefab, transform.position, Quaternion.identity);
            dome.GetComponent<StunDomeVisual>().Play(stunRadius);
        }

        Destroy(gameObject, 0.1f);
    }
}
