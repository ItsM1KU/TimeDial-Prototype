using System.Collections.Generic;
using UnityEngine;

public class DecoyClone : MonoBehaviour
{
    [SerializeField] private float aggroRadius = 15f; 
    private float lifetime;
    private int killCount = -1; 

    private readonly List<MonoBehaviour> aggroedEnemies = new List<MonoBehaviour>();

    public void Init(float duration, int killCountOnDestroy)
    {
        lifetime = duration;
        killCount = killCountOnDestroy;

        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Zombie zombie))
            {
                zombie.SetTarget(transform);
                aggroedEnemies.Add(zombie);
            }
            else if (hit.TryGetComponent(out Hunter hunter))
            {
                hunter.SetTarget(transform);
                aggroedEnemies.Add(hunter);
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void OnDestroy()
    {
        // Return aggro to the player
        foreach (var enemy in aggroedEnemies)
        {
            if (enemy == null) continue;
            if (enemy is Zombie z) z.ClearTarget();
            if (enemy is Hunter h) h.ClearTarget();
        }

        if (killCount >= 0)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius);
            int killed = 0;
            foreach (var hit in hits)
            {
                if (killed >= killCount) break;

                if (hit.TryGetComponent(out Zombie z) || hit.TryGetComponent(out Hunter h))
                {
                    Destroy(hit.gameObject);
                    killed++;
                }
            }
        }
    }
}
