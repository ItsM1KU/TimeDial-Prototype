using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody rb;

    private float lifetime = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (lifetime > 0f)
        {
            lifetime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent(out PlayerHealth playerHealth))
            {
                Debug.Log("Player hit by enemy projectile");
                playerHealth.TakeDamage(5f); 
            }

            Destroy(gameObject);
        }
    }
}
