using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Attack : MonoBehaviour
{
    [SerializeField] protected LayerMask attackableLayers;
    [SerializeField] protected float damage;
    [SerializeField] protected float damageTickCooldown = 0.2f;

    // Intentional "bug" where the attack deals damage again if the target leaves then re-enters its hitbox.
    [SerializeField] private bool repeatedHitBug;

    // Stores the last point in time the attack connected, for each individual target.
    protected readonly Dictionary<GameObject, float> lastHitTimes = new();

    protected Collider2D hitbox;

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
    }

    public void SetActive(bool active)
    {
        hitbox.enabled = active;
        lastHitTimes.Clear();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // If intentional repeated hit "bug" is active, skip the check of whether target has been hit recently.
        var cachedDmgTickCd = damageTickCooldown;
        if (repeatedHitBug) damageTickCooldown = 0f;
        TryHitTarget(collision);
        damageTickCooldown = cachedDmgTickCd;
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        TryHitTarget(collision);
    }

    protected virtual void TryHitTarget(Collider2D collision)
    {
        if ((attackableLayers & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        if (lastHitTimes.TryGetValue(collision.gameObject, out var timeTargetLastHit)
           && timeTargetLastHit + damageTickCooldown > Time.time)
        {
            return;
        }
        lastHitTimes[collision.gameObject] = Time.time;

        if (collision.transform.parent.TryGetComponent<IDamageable>(out var target))
        {
            var hitDirection = GetHitDirection(collision);
            DealDamage(target, hitDirection);
        }
    }

    protected virtual Vector2 GetHitDirection(Collider2D target)
    {
        return (target.transform.position - transform.position).normalized;
    }

    protected virtual void DealDamage(IDamageable target, Vector2 hitDirection)
    {
        target.TakeDamage(damage, hitDirection);
    }
}
