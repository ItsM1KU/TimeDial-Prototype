using UnityEngine;

public class PlayerProjectile : MonoBehaviour
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
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out Zombie zombie))
            {
                zombie.TakeDamage(10f); 
            }

            if(other.TryGetComponent(out Hunter hunter))
            {
                hunter.TakeDamage(10f); 
            }
            Destroy(gameObject);
        }
    }
}
