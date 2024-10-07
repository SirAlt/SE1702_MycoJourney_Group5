using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(BodyContacts), typeof(IMovementStrategy))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [field: SerializeField] public PlayerStats Stats { get; set; }
    [field: SerializeField] public PlayerAbilities Abilities { get; set; }

    #region IDamageable

    [field: SerializeField] public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion

    #region State Machine

    public PlayerStateMachine StateMachine { get; private set; }

    #region States

    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }

    public PlayerGroundJumpState GroundJumpState { get; private set; }
    public PlayerAirJumpState AirJumpState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }

    public PlayerJumpFallState JumpFallState { get; private set; }
    public PlayerNaturalFallState NaturalFallState { get; private set; }

    public PlayerWallSlideState WallSlideState { get; private set; }

    public PlayerGroundDashState GroundDashState { get; private set; }
    public PlayerAirDashState AirDashState { get; private set; }

    #endregion

    #region Helpers

    public float TimeLeftGround { get; set; }   // Coyote time
    public float TimeJumpStarted { get; set; }  // Initial jump period + Variable jump height + Anti-mash + Wall-jump initial path

    private bool HasJumpInput => Input.JumpPressedThisFrame || (Input.TimeJumpWasPressed + Stats.JumpBuffer > Time.time);
    private bool IsInJumpRefractoryPeriod => TimeJumpStarted + Stats.JumpRefractoryPeriod > Time.time;
    public bool HasValidJumpInput => HasJumpInput && !IsInJumpRefractoryPeriod;

    public bool IsMovingAgainstWall => (BodyContacts.WallLeft && Input.Move.x < 0) || (BodyContacts.WallRight && Input.Move.x > 0);

    public float TimeDashStarted { get; set; }  // Dash duration + Variable dash length
    public float TimeDashEnded { get; set; }    // Dash cooldown

    private bool HasDashInput => Input.DashPressedThisFrame || (Input.TimeDashWasPressed + Stats.DashBuffer > Time.time);
    private bool DashOnCooldown => TimeDashEnded + Abilities.DashCooldown > Time.time;
    public bool HasValidDashInput => Abilities.DashLearnt && HasDashInput && !DashOnCooldown;

    public bool IsFacingRight => transform.rotation.eulerAngles.y == 0f;

    public void FaceLeft() => transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    public void FaceRight() => transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    public void Land()
    {
        if (Input.Move.x != 0)
            StateMachine.ChangeState(RunState);
        else
            StateMachine.ChangeState(IdleState);
    }

    #endregion

    #endregion

    #region Animator

    public Animator Animator { get; private set; }

    public const string IdleAnim = "Idle";
    public const string RunAnim = "Run";
    public const string GroundJumpAnim = "Jump";
    public const string AirJumpAnim = "Jump";
    public const string WallJumpAnim = "Jump";
    public const string NaturalFallAnim = "Fall";
    public const string JumpFallAnim = "Fall";
    public const string WallSlideAnim = "WallSlide";
    public const string GroundDashAnim = "Dash";
    public const string AirDashAnim = "Dash";

    public const string FlinchAnim = "Flinch";
    public const string DeathAnim = "Death";

    public enum AnimationTriggerType
    {
        AttackEnd,
        Flinch,
        DeathFinished,
    }

    private void NotifyAnimationEventTriggered(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentState.OnAnimationEventTriggered(triggerType);
    }

    #endregion

    #region Abilities

    public float AirJumpCharges { get; set; }

    public float AirDashCharges { get; set; }

    #endregion

    public InputManager Input { get; private set; }
    public BodyContacts BodyContacts { get; private set; }

    // Exposing a field is ugly, but better than the boilerplate that is the alternative.
    // It was Unity that forced a mutable struct on us.
    [HideInInspector] public Vector2 FrameVelocity;

    private IMovementStrategy _moveStrat;
    private Rigidbody2D _rb;

    private void Awake()
    {
        BodyContacts = GetComponent<BodyContacts>();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        GroundJumpState = new PlayerGroundJumpState(this, StateMachine);
        AirJumpState = new PlayerAirJumpState(this, StateMachine);
        WallJumpState = new PlayerWallJumpState(this, StateMachine);
        JumpFallState = new PlayerJumpFallState(this, StateMachine);
        NaturalFallState = new PlayerNaturalFallState(this, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, StateMachine);
        GroundDashState = new PlayerGroundDashState(this, StateMachine);
        AirDashState = new PlayerAirDashState(this, StateMachine);

        _moveStrat = GetComponent<IMovementStrategy>();
        _rb = GetComponent<Rigidbody2D>();

        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Input = InputManager.Instance;
        CurrentHealth = MaxHealth;
        StateMachine.Initialize(IdleState);
        _moveStrat.Rb = _rb;
    }

    private void Update()
    {
        StateMachine.FrameUpdate();
    }

    #region Physics

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        _moveStrat.Move(FrameVelocity);
    }

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Stats == null) Debug.LogWarning($"Please assign a {nameof(PlayerStats)} asset to the Player Controller's Stats slot", this);
        if (Abilities == null) Debug.LogWarning($"Please assign a {nameof(PlayerAbilities)} asset to the Player Controller's Abilties slot", this);
    }
#endif
}
