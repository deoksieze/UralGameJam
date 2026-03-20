using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Tuning")]
    public float attackDuration = 0.2f;
    public float knockbackForce = 5f;
    public int damage = 1;
    public int maxHealth = 5;
    public float knockbackTime = 0.2f;

    [Header("Components")]
    public PlayerHealth health;
    public PlayerCombat combat;
    public PlayerMovement movement;


    void Awake()
    {
        if (!movement) movement = GetComponent<PlayerMovement>();
        if (!combat)   combat   = GetComponent<PlayerCombat>();
        if (!health)   health   = GetComponent<PlayerHealth>();

        combat.attackDuration = attackDuration;
        combat.damage = damage;
        combat.knockbackForce = knockbackForce;

        health.SetHp(maxHealth);
        health.knockbackTime = knockbackTime;


    }
}
