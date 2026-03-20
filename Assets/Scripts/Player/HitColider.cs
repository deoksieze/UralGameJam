using System.Threading.Tasks;
using UnityEngine;

public class HitColider : MonoBehaviour
{
    public PlayerCombat playerCombat;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var Enemy = other.GetComponent<Enemy>();

        Vector2 dir = (other.transform.position - transform.root.position).normalized;

        Enemy.TakeDamage(playerCombat.damage, dir*playerCombat.knockbackForce);
    }
}
