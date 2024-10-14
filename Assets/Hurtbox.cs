using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hurtbox : MonoBehaviour
{
    [HideInInspector] public BoxCollider2D Collider;

    private void Awake()
    {
        Collider = GetComponent<BoxCollider2D>();
    }

    public void BeginInvincibility(float duration = -1)
    {
        DisableHurtbox();
        if (duration > 0) Invoke(nameof(EnableHurtbox), duration);
    }

    public void EndInvincibility()
    {
        EnableHurtbox();
    }

    public void Rescale(Vector2 scale)
    {
        Collider.size = scale;
    }

    private void EnableHurtbox() => Collider.enabled = true;
    private void DisableHurtbox() => Collider.enabled = false;
}
