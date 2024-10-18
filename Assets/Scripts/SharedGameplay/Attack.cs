using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] protected LayerMask attackableLayers;
    [SerializeField] protected float damage;
    [SerializeField] protected bool multiHit;
    [SerializeField] protected float damageTickCooldown;
    
    [SerializeField] private bool doubleDamageBug;

    protected float damageTickTimer;
    protected Vector2 hitDirection;
    protected Collider2D hitbox;

    // Intentional "bug" where the attack deals damage again if the target leaves then re-enters its hitbox.

    protected virtual void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;
    }

    protected virtual void Start()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (!multiHit || damageTickTimer <= 0) return;
        damageTickTimer -= Time.fixedDeltaTime;
    }

    public void Activate() => hitbox.enabled = true;
    public void Deactivate() => hitbox.enabled = false;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (doubleDamageBug || damageTickTimer <= 0)
        {
            FindTargetAndDealDamage(collision);
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (damageTickTimer <= 0)
        {
            FindTargetAndDealDamage(collision);
        }
    }

    protected virtual void FindTargetAndDealDamage(Collider2D col)
    {
        if ((attackableLayers & (1 << col.gameObject.layer)) != 0
            && col.transform.parent.TryGetComponent<IDamageable>(out var target))
        {
            damageTickTimer = damageTickCooldown;
            hitDirection = (col.transform.position - transform.position).normalized;
            target.TakeDamage(damage, hitDirection);
        }
    }
}
