using System;
using UnityEngine;

public class HealthPoints : MonoBehaviour
{
    public event Action OnDied;
    public event Action OnTakenDamage;

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;

    public float GetCurrentHealth { get { return currentHealth; } }
    public float GetMaxHealth { get { return maxHealth; } }

    public HealthPoints(float currentHealth, float maxHealth)
    {
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        OnTakenDamage?.Invoke();

        if (currentHealth <= 0f)
        {
            OnDied?.Invoke();
        }
    }
}
