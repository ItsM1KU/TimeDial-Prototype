using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] private GameManager gameManager;

    public event Action<float, float> OnHealthChanged;

    private void Awake() => currentHealth = maxHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Cursor.visible = true;
        gameManager.EndGame();
    }
}
