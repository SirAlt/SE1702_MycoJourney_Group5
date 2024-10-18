using UnityEngine;

public class Flamethrower : Attack
{
    [SerializeField] private float _activeDuration;
    [SerializeField] private float _dormantDuration;

    private SpriteRenderer _sprite;
    private Animator _animator;

    private float _timer;
    private bool _active;

    protected override void Awake()
    {
        base.Awake();

        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        if (_activeDuration == 0)
            FlameOff();
        else
            FlameOn();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if ((!_active && _activeDuration == 0)
            || (_active && _dormantDuration == 0))
        {
            return;
        }

        _timer -= Time.fixedDeltaTime;
        if (_timer <= 0)
        {
            if (_active)
                FlameOff();
            else
                FlameOn();
        }
    }

    private void FlameOn()
    {
        _active = true;
        _timer = _activeDuration;
        _sprite.enabled = true;
        _animator.enabled = true;
        hitbox.enabled = true;
    }

    private void FlameOff()
    {
        _active = false;
        _timer = _dormantDuration;
        _sprite.enabled = false;
        _animator.enabled = false;
        hitbox.enabled = false;
    }
}
