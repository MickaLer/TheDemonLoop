using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float startingHealth = 100f;
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }

    public Action<float> OnHealthChanged;
    public Action OnDeath;

    private void Awake()
    {
        MaxHealth = startingHealth;
        CurrentHealth = startingHealth;
    }
    
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        OnHealthChanged?.Invoke(CurrentHealth);
        if(CurrentHealth <= 0) OnDeath?.Invoke();
    }

    public void ChangeMaxLife(float maxLife, bool fullHeal = true)
    {
        MaxHealth = maxLife;
        if(fullHeal) CurrentHealth = MaxHealth;
    }
}