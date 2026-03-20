using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 5;
    public float knockBackTime = 3f;

    public Rigidbody2D rb;
    public MovingObject mv;

    [Header("Attack")]
    [SerializeField] Collider2D attackCollider;   // коллайдер удара (Trigger)
    [SerializeField] float attackInterval = 1f;   // раз в секунду
    [SerializeField] float attackActiveTime = 0.5f;

    void Start()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
            StartCoroutine(AttackLoop());
        }
    }
    
    public void TakeDamage(int amount, Vector2 knockBack)
    {
        health -= amount;
        Debug.Log("Enemy took: " + amount + " damage. Health remain: " + health);

        StartCoroutine(DoKnockback(knockBack));

        if (health <= 0)
        {
            Debug.Log("Enemy is Dead");
            Destroy(gameObject);
        }
    }

    IEnumerator DoKnockback(Vector2 knockBack)
    {
        mv.SetKnockBack(true);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockBack, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackTime);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        mv.SetKnockBack(false);
    }


    IEnumerator AttackLoop()
    {
        while (true)
        {
            // ждём до следующей атаки
            yield return new WaitForSeconds(attackInterval);

            // включаем хитбокс
            attackCollider.enabled = true;


            // держим активным полсекунды
            yield return new WaitForSeconds(attackActiveTime);

            attackCollider.enabled = false;
        }
    }
}
