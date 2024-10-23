using UnityEngine;

public class EnemyFlyingEye : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    void Update()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("PlyingEyeHit")
            && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            _anim.Play("PlyingEyeHitFly", -1, 0f);
        }
    }

    public void TakeDamage(float damage) => TakeDamage(damage, Vector2.zero);

    public void TakeDamage(float damage, Vector2 direction)
    {
        CurrentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, remaining health: {CurrentHealth}");
        CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("PlyingEyeHit"))
        {
            _anim.Play("PlyingEyeHit", -1, 0f);
        }

        if (CurrentHealth <= 0)
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
