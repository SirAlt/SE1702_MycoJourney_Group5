using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hurtbox : MonoBehaviour
{
    public BoxCollider2D Collider { get; private set; }

    public bool _hasInvincibility;
    private float _timeToEndInvincibility;

    private void Awake()
    {
        Collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (_hasInvincibility && Time.time >= _timeToEndInvincibility)
        {
            RemoveInvincibility();
        }
    }

    public void GainInvincibility(float duration = -1.0f)
    {
        if (duration == 0f) return;

        if (!_hasInvincibility)
        {
            _hasInvincibility = true;
            Collider.enabled = false;
        }

        if (duration < 0f)
        {
            _timeToEndInvincibility = Mathf.Infinity;
        }
        else if (Time.time + duration > _timeToEndInvincibility)
        {
            _timeToEndInvincibility = Time.time + duration;
        }
    }

    public void RemoveInvincibility()
    {
        _hasInvincibility = false;
        _timeToEndInvincibility = 0f;
        Collider.enabled = true;
    }
}
