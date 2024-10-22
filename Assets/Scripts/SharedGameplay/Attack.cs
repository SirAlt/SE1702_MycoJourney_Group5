using Assets.Scripts.Player.Events;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private LayerMask attackableLayers;
    [SerializeField] private float damage;

    private Collider2D _hitbox;

    private void Awake()
    {
        _hitbox = GetComponent<Collider2D>();
        _hitbox.isTrigger = true;
        _hitbox.enabled = false;
    }

    public void Activate() => _hitbox.enabled = true;
    public void Deactivate() => _hitbox.enabled = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((attackableLayers & (1 << other.gameObject.layer)) != 0)
        {
            var target = other.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);
     
        }
    }
}
