using System;
using UnityEngine;

public class SimpleDamageDealer : MonoBehaviour
{
    public GameObject owner;
    public float damages;
    public bool destroyWhenHit;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Damageable damageable) && other.gameObject != owner) 
            damageable.TakeDamage(damages);
        if(destroyWhenHit) Destroy(gameObject);
    }
}
