using UnityEngine;

public class Hazard : Attack
{
    [SerializeField] protected bool instantDeath;

    protected override void Start()
    {
        //base.Start();
        SetActive(true);
    }

    protected override void DealDamage(IHitReceptor target, Vector2 force)
    {
        if (instantDeath)
            target.TakeHit(9999f, Vector2.zero, IHitReceptor.HitAttributes.InstantKill | IHitReceptor.HitAttributes.PiercesInvincibility);
        else
            base.DealDamage(target, force);
    }
}
