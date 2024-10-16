using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(InputManager), typeof(BodyContacts))]
[RequireComponent(typeof(IMovement), typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IMoveable, IDamageable
{
    [field: SerializeField] public PlayerStats Stats { get; private set; }
    [field: SerializeField] public PlayerAbilities Abilities { get; private set; }

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
    public PlayerDropThroughFallState DropThroughFallState { get; private set; }

    public PlayerWallSlideState WallSlideState { get; private set; }

    public PlayerGroundDashState GroundDashState { get; private set; }
    public PlayerAirDashState AirDashState { get; private set; }

    public PlayerStandSlashState StandSlashState { get; private set; }
    public PlayerRunSlashState RunSlashState { get; private set; }
    public PlayerAirSlashState AirSlashState { get; private set; }
    public PlayerDashSlashState DashSlashState { get; private set; }

    #endregion

    #region Helpers

    public float TimeLeftGround { get; set; } = Mathf.NegativeInfinity;   // Coyote time
    public float TimeJumpStarted { get; set; } = Mathf.NegativeInfinity;  // Initial jump period + Variable jump height + Anti-mash + Wall-jump initial path

    public bool HasJumpInput => Input.JumpPressedThisFrame || (Input.TimeJumpWasPressed + Stats.JumpBuffer > Time.time);
    public bool IsInJumpRefractoryPeriod => TimeJumpStarted + Stats.JumpRefractoryPeriod > Time.time;
    public bool HasValidJumpInput => HasJumpInput && !IsInJumpRefractoryPeriod;

    public bool IsMovingAgainstWall => (BodyContacts.WallLeft && Input.Move.x < 0) || (BodyContacts.WallRight && Input.Move.x > 0);

    public float TimeLeftWall { get; set; } = Mathf.NegativeInfinity;   // Wall jump coyote time
    public int LastWallContactSide { get; set; }    // -1 = Left; 1 = Right

    public float TimeDashStarted { get; set; } = Mathf.NegativeInfinity; // Dash duration + Stop dash
    public float TimeDashEnded { get; set; } = Mathf.NegativeInfinity;    // Dash cooldown

    public bool HasDashInput => Input.DashPressedThisFrame || (Input.TimeDashWasPressed + Stats.DashBuffer > Time.time);
    public bool DashOnCooldown => TimeDashEnded + Abilities.DashCooldown > Time.time;
    public bool HasValidDashInput => Abilities.DashLearnt && HasDashInput && !DashOnCooldown;

    public float TimeSlashActivated { get; set; } = Mathf.NegativeInfinity; // Attack speed

    public bool HasSlashInput => Input.SlashPressedThisFrame || (Input.TimeSlashWasPressed + Stats.SlashBuffer > Time.time);
    public bool SlashOnCooldown => TimeSlashActivated + Abilities.SlashCooldown > Time.time;
    public bool HasValidSlashInput => HasSlashInput && !SlashOnCooldown;

    public float TimeDropThroughStarted { get; set; } = Mathf.NegativeInfinity;

    public bool IsFacingRight => transform.rotation.eulerAngles.y == 0f;

    public bool IsTurningAround => (IsFacingRight && Input.Move.x < 0) || (!IsFacingRight && Input.Move.x > 0);

    public void FaceLeft() => transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    public void FaceRight() => transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    public void Land()
    {
        if (Input.Move.x != 0)
            StateMachine.ChangeState(RunState);
        else
            StateMachine.ChangeState(IdleState);
    }

    public void LandImmediate()
    {
        if (Input.Move.x != 0)
            StateMachine.ChangeStateImmediate(RunState);
        else
            StateMachine.ChangeStateImmediate(IdleState);
    }

    #endregion

    #endregion

    #region FX

    public Animator Animator { get; private set; }
    public PlayerFX FX { get; private set; }

    public const string IdleAnim = "Idle";
    public const string RunAnim = "Run";

    public const string GroundJumpAnim = "Jump";
    public const string AirJumpAnim = "Jump";
    public const string WallJumpAnim = "Jump";

    public const string NaturalFallAnim = "Fall";
    public const string JumpFallAnim = "Fall";

    public const string WallSlideAnim = "WallSlide";

    public const string GroundDashStartAnim = "Dash Start";
    public const string GroundDashEndAnim = "Dash End";
    public const string AirDashStartAnim = "Dash Start";
    public const string AirDashEndAnim = "Dash End";

    public const string StandSlashAnim = "Attack";
    public const string RunSlashAnim = "Attack Move";
    public const string AirSlashAnim = "Jump Attack";
    public const string DashSlashAnim = "Dash Attack";

    public const string FlinchAnim = "Flinch";
    public const string DeathAnim = "Death";

    public enum AnimationTriggerType
    {
        AttackActiveFramesStarted,
        AttackActiveFramesEnded,
        AttackFinished,
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

    #region Attacks

    [field: SerializeField] public Attack StandSlash { get; private set; }
    [field: SerializeField] public Attack RunSlash { get; private set; }
    [field: SerializeField] public Attack AirSlash { get; private set; }
    [field: SerializeField] public Attack DashSlash { get; private set; }

    #endregion

    #region IMoveable

    public void Move(Vector2 velocity)
    {
        Mover.EnvironmentVelocity += velocity;
    }

    #endregion

    #region IDamageable

    [field: SerializeField] public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

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

    public InputManager Input { get; private set; }
    public BodyContacts BodyContacts { get; private set; }
    public BoxCollider2D CollisionBox { get; private set; }
    public Hurtbox Hurtbox { get; private set; }

    // Exposing a field is ugly, but better than the boilerplate that is the alternative.
    // It was Unity that forced a mutable struct on us.
    [HideInInspector] public Vector2 FrameVelocity;

    [HideInInspector] public IMovement Mover { get; private set; }

    private void Awake()
    {
        BodyContacts = GetComponent<BodyContacts>();
        Hurtbox = GetComponentInChildren<Hurtbox>();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        GroundJumpState = new PlayerGroundJumpState(this, StateMachine);
        AirJumpState = new PlayerAirJumpState(this, StateMachine);
        WallJumpState = new PlayerWallJumpState(this, StateMachine);
        JumpFallState = new PlayerJumpFallState(this, StateMachine);
        NaturalFallState = new PlayerNaturalFallState(this, StateMachine);
        DropThroughFallState = new PlayerDropThroughFallState(this, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, StateMachine);
        GroundDashState = new PlayerGroundDashState(this, StateMachine);
        AirDashState = new PlayerAirDashState(this, StateMachine);
        StandSlashState = new PlayerStandSlashState(this, StateMachine);
        RunSlashState = new PlayerRunSlashState(this, StateMachine);
        AirSlashState = new PlayerAirSlashState(this, StateMachine);
        DashSlashState = new PlayerDashSlashState(this, StateMachine);

        Animator = GetComponent<Animator>();
        FX = GetComponent<PlayerFX>();

        Mover = GetComponent<IMovement>();
        CollisionBox = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        Input = InputManager.Instance;
        StateMachine.Initialize(IdleState);
        Mover.Collider = CollisionBox;
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        StateMachine.FrameUpdate();
    }

    #region Physics

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
        Mover.Move(FrameVelocity);
        Mover.EnvironmentVelocity = Vector2.zero;
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
