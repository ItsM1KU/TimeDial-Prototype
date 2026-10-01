using UnityEngine;

public class SkillOrb : MonoBehaviour
{
    [SerializeField] private float cooldownReduction = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerSkillController skillController))
        {
            skillController.ReduceCooldown(cooldownReduction);
        }

        Destroy(gameObject);
    }
}
