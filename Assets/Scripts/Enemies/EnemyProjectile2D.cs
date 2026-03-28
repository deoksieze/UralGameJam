using UnityEngine;

public class EnemyProjectile2D : MonoBehaviour
{
    public Rigidbody2D rb;
    public float lifeTime = 4f;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            return;

        if (other.isTrigger && !other.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }
}