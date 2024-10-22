using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hurtbox : MonoBehaviour
{
    public BoxCollider2D Collider { get; private set; }
    public bool HasInvincibility { get; private set; }

    private float _timeToEndInvincibility;

    private void Awake()
    {
        Collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (HasInvincibility && Time.time >= _timeToEndInvincibility)
        {
            HasInvincibility = false;
        }
    }

    public void GainInvincibility(float duration = 0f)
    {
        if (!HasInvincibility) HasInvincibility = true;

        if (duration < 0f)
        {
            _timeToEndInvincibility = Mathf.Infinity;
        }
        else if (Time.time + duration > _timeToEndInvincibility)
        {
            _timeToEndInvincibility = Time.time + duration;
        }
    }

    public void RemoveInvincibility() => HasInvincibility = false;
}
