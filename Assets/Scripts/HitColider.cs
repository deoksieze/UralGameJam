using System.Threading.Tasks;
using UnityEngine;

public class HitColider : MonoBehaviour
{
    public int damage = 1;
    public float knockBackForce = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var Enemy = other.GetComponent<Enemy>();

        Vector2 dir = (other.transform.position - transform.root.position).normalized;

        Enemy.TakeDamage(damage, dir*knockBackForce);
    }
}
