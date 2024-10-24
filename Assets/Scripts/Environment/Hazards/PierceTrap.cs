using UnityEngine;

public class PierceTrap : Hazard, ITriggerable
{
    private const string TriggeredAnim = "Triggered";
    private const string ExtendAnim = "Extend";
    private const string RetractAnim = "Retract";

    [SerializeField] private float tellTime;
    [SerializeField] private float stayTime;
    [SerializeField] private float rearmTime;
    [SerializeField] private Collider2D _detector;

    private Animator _anim;

    protected override void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        //base.Start();
        SetActive(false);
    }

    void ITriggerable.Trigger()
    {
        _detector.enabled = false;
        _anim.Play(TriggeredAnim, -1, 0f);
        Invoke(nameof(Stab), tellTime);
        Invoke(nameof(Retract), tellTime + stayTime);
    }

    void ITriggerable.Reset()
    {
        Retract();
    }

    private void Stab()
    {
        // TODO: Multiple hitbox stages.
        SetActive(true);
        _anim.Play(ExtendAnim, -1, 0f);
    }

    private void Retract()
    {
        // TODO: Multiple hitbox stages.
        SetActive(false);
        _anim.Play(RetractAnim, -1, 0f);
    }

    // AnimEvent: Retract [End]
    public void OnFullyRetracted()
    {
        Invoke(nameof(Rearm), rearmTime);
    }

    private void Rearm()
    {
        _detector.enabled = true;
    }
}
