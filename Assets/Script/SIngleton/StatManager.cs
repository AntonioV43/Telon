using UnityEngine;
using System;

public class StatManager : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log($"{gameObject.name} healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} health reset: {currentHealth}/{maxHealth}");
    }
}