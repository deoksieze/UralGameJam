using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int maxHealth;
    public int health;
    public int maxStamina;
    public int currentStamina;

    public float damage;

    public Action OnDeath;

    void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log("Damage taken : " + dmg + " current hp: " + health);
        if (health <= 0)
        {
            OnDeath.Invoke();
        }
    }

    
}
