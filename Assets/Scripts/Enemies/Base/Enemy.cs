using Assets.Scripts.Player.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{


     private float health = 100f;  
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth => health;

    public float MaxHealth => maxHealth;

    public void TakeDamage(float damage)
    {
        health -= damage; 
        Debug.Log($"{gameObject.name} took {damage} damage, remaining health: {health}");
        CharacterEvents.characterDamaged?.Invoke(gameObject, damage);
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
