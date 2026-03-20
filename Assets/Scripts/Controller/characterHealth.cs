using System.Collections;
using UnityEngine;

public class characterHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] characterMovement movement;
    [SerializeField] characterJump jump;
    [SerializeField] float knockbackTime = 1f;
    int currentHealth;
    bool isBlocking;        // сюда ещё вернёмся

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetBlocking(bool value)
    {
        isBlocking = value;
    }

    public void TakeDamage(int amount, Vector2 knockback, bool isBlocked)
    {
        if (isBlocked)
        {
            // тут можно уменьшить/обнулить урон
            amount /= 2; // или amount /= 2 и т.п.
            knockback *= 0.4f;
        }

        if (amount <= 0) return;

        StartCoroutine(DoKnockback(knockback*10));

        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    IEnumerator DoKnockback(Vector2 knockback)
    {
        SetKnockback(true);

        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        Debug.Log("knockbacl vector" + knockback);
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        rb.linearVelocity = Vector2.zero;

        SetKnockback(false);
    }

    void Die()
    {
        Debug.Log("Player is dead");
        // анимация смерти, отключение управления и т.п.
    }

    void SetKnockback(bool flag)
    {
        movement.isKnockback = flag;
        jump.isKnockback = flag;
    }
}
