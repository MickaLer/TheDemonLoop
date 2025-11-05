using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private float invulnerabilityFrames = 0f;
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }

    public Action<float> OnHealthChanged;
    public Action OnDeath;

    private float _invulnerabilityTimer;
    
    private void Awake()
    {
        MaxHealth = startingHealth;
        CurrentHealth = startingHealth;
    }

    private void Update()
    {
        _invulnerabilityTimer += Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (invulnerabilityFrames > _invulnerabilityTimer) return;
        Debug.Log(gameObject.name + " took " + damage + " damage");
        CurrentHealth -= damage;
        OnHealthChanged?.Invoke(CurrentHealth);
        if(CurrentHealth <= 0) OnDeath?.Invoke();
        _invulnerabilityTimer = 0;
    }

    public void ChangeMaxLife(float maxLife, bool fullHeal = true)
    {
        MaxHealth = maxLife;
        if(fullHeal) CurrentHealth = MaxHealth;
    }
}