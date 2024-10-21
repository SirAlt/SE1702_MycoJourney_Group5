using UnityEngine;

public class Hazard : Attack
{
    [SerializeField] protected bool instantDeath;

    protected override void Start()
    {
        //base.Start();
        SetActive(true);
    }

    protected override void DealDamage(IDamageable target, Vector2 hitDirection)
    {
        if (instantDeath)
            target.Die();
        else
            base.DealDamage(target, hitDirection);
    }
}
