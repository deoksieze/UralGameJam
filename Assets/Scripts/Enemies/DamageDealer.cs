using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 4;
    public bool isProjectile = false;
    public float knockbackForce = 5f;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;
        
        var combat = other.GetComponent<PlayerCombat>();

        bool blocked = combat != null && combat.IsThisDealerBlocked(this);

        Vector2 dir = (other.transform.position - transform.root.position).normalized;
        Vector2 knockback = dir * 5f;

        if (blocked)
        {

            playerHealth.TakeDamage(damage, knockback, true);

            // 2) разное поведение для пули и мелее
            if (isProjectile)
            {
                Destroy(gameObject); // снаряд гасим
            }
            
            Debug.Log("Hit blocked by shield zone");
        }
        else
        {
            // Обычный урон
            playerHealth.TakeDamage(damage, knockback, false);
        }
        
    }
}

