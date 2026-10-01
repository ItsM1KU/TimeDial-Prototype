using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                Debug.Log("Player collected health orb");
                playerHealth.Heal(healAmount);
            }
            Destroy(gameObject);
        }
    }
}
