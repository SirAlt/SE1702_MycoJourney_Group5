public interface IDamageable
{
    float CurrentHealth { get; }
    float MaxHealth { get; }

    void TakeDamage(float damage);

    void Die();
}
