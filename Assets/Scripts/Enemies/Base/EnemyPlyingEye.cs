using Assets.Scripts.Player.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlyingEye : MonoBehaviour, IDamageable
{

    Animator animator;

    private float health = 100f;
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth => health;

    public float MaxHealth => maxHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlyingEyeHit"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
               
                animator.Play("PlyingEyeHitFly", -1, 0f);
            }
        }
    }

   

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, remaining health: {health}");
        CharacterEvents.characterDamaged?.Invoke(gameObject, damage);
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlyingEyeHit"))
        {
            animator.Play("PlyingEyeHit", -1, 0f);
        }
        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
}
