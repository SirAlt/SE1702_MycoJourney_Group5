using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(BodyContacts), typeof(IMovementStrategy))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerAbilities _abilities;

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

    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }

    #endregion

    #region Animator

    public Animator Animator { get; private set; }

    public enum AnimationTriggerType
    {
        Run,
        Jump,
        Fall,
        Hurt,
        Death,
    }

    private void AnimationEventTriggered(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentState.TriggerAnimation(triggerType);
    }

    #endregion

    public InputManager Input { get; private set; }

    public BodyContacts BodyContacts { get; private set; }

    private IMovementStrategy _movementStrategy;
    private Rigidbody2D _rb;

    private void Awake()
    {
        Input = InputManager.Instance;

        BodyContacts = GetComponent<BodyContacts>();

        _movementStrategy = GetComponent<IMovementStrategy>();
        _rb = GetComponent<Rigidbody2D>();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        FallState = new PlayerFallState(this, StateMachine);

        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;

        _movementStrategy.Rb = _rb;

        StateMachine.Initialize(IdleState);
    }

    private float _time;

    private void Update()
    {
        _time += Time.deltaTime;
        ProcessInput();
        StateMachine.CurrentState.FrameUpdate();
    }

    private void ProcessInput()
    {
        if (Input.JumpPressedThisFrame)
        {
            _jumpPressedThisFrame = true;
            _timeJumpWasPressed = _time;
        }
    }

    #region Physics

    private Vector2 _frameVelocity;
    public Vector2 FrameVelocity
    {
        get => _frameVelocity;
        set => _frameVelocity = value;
    }

    private void FixedUpdate()
    {
        Setup();

        GetApexRatio();

        CheckWall();
        HandleMovement();

        CheckGround();
        CheckCeiling();
        HandleAirControl();
        HandleGravity();
        HandleJump();

        ApplyMovement();

        Cleanup();

        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void Setup()
    {
        Debug.Log($"========== FixedUpdate: Time={Time.time} ==========");
        Debug.Log($"Starting position: {transform.position}");
        Debug.Log($"Starting frame velocity: {_frameVelocity}");
    }

    private void Cleanup()
    {
        _jumpPressedThisFrame = false;
        Debug.Log($"Final position: {transform.position}");
        Debug.Log($"Final frame velocity: {_frameVelocity}");
        Debug.Log($"========== ******************** ==========");
    }

    #region Apex Modifiers

    private float _apexRatio;

    private void GetApexRatio()
    {
        if (!_isJumping)
            _apexRatio = 0;
        else
            _apexRatio = Mathf.InverseLerp(_stats.ApexThreshold, 0, Mathf.Abs(_frameVelocity.y));

        //Debug.Log($"ApexRatio: {_apexRatio}");
    }

    #endregion

    private bool _isOnWall;

    private bool IsTouchingWall => BodyContacts.IsTouchingWall;
    private bool IsMovingAgainstWall => (BodyContacts.IsTouchingWallRight && Input.MoveInput.x > 0) || (BodyContacts.IsTouchingWallLeft && Input.MoveInput.x < 0);

    private void CheckWall()
    {
        // Grabbed wall
        if (!_isOnWall && IsMovingAgainstWall && _frameVelocity.y <= 0)
        {
            _isOnWall = true;
            _isJumping = false;
            _endedJumpEarly = false;
            _airJumpCharges = _abilities.AirJumpCharges;
            // Notify Animator
        }
        // Left wall
        else if (_isOnWall && (!IsMovingAgainstWall || _frameVelocity.y > 0))
        {
            _isOnWall = false;
            // Notify Animator
        }
    }

    private void HandleMovement()
    {
        if (Input.MoveInput.x == 0)
        {
            var deceleration = _isGrounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _stats.MaxHorizontalSpeed * Mathf.Sign(Input.MoveInput.x), _stats.HorizontalAcceleration * Time.fixedDeltaTime);
            if (_isJumping)
            {
                var apexVelocityBonus = _stats.ApexAccelerationBonus * _apexRatio * Time.fixedDeltaTime;
                _frameVelocity.x += apexVelocityBonus * Mathf.Sign(Input.MoveInput.x);
                //Debug.Log($"ApexVelocityBonus: {apexVelocityBonus}");
            }

            if (Input.MoveInput.x < 0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            else transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private bool _isGrounded;

    private void CheckGround()
    {
        bool groundHit = BodyContacts.IsTouchingGround;
        //Debug.Log($"GroundCheck: {groundHit}");

        // Landed
        if (!_isGrounded && groundHit)
        {
            _isGrounded = true;
            _isOnWall = false;
            _isJumping = false;
            _endedJumpEarly = false; // If we don't unset this here, we might run into issues when the character moves upward without jumping
            _airJumpCharges = _abilities.AirJumpCharges;
            // Notify Animator
        }
        // Left ground
        else if (_isGrounded && !groundHit)
        {
            _isGrounded = false;
            _timeLeftGround = _time;
            // Notify Animator
        }
    }

    private void CheckCeiling()
    {
        bool ceilingHit = BodyContacts.IsTouchingCeiling;
        if (ceilingHit) _frameVelocity.y = Mathf.Min(_frameVelocity.y, 0);
    }

    #region Gravity

    private void HandleAirControl()
    {
        // 1. Because we have input buffering, we cannot rely on the keyup event to set this flag. The player could have already released the Jump key before even hitting the ground.
        // 2. We set this value once, and only unset it upon entering or exiting the jump state. This means a jump can be ended early exactly once, and never "un-ended".
        if (!_endedJumpEarly
            && _isJumping
            && !Input.JumpHeld
            && _frameVelocity.y > 0
            && _timeJumpStarted + _stats.JumpEndEarlyWindow > _time
          )
        {
            _endedJumpEarly = true;
        }
    }

    private void HandleGravity()
    {
        if (_isGrounded)
        {
            _frameVelocity.y = -_stats.GroundingVelocity;
        }
        else if (_isOnWall && _abilities.WallSlideLearnt)
        {
            _frameVelocity.y = -_abilities.WallSlideGravityModifier * _stats.GravitationalAcceleration * Time.fixedDeltaTime;
        }
        else
        {
            var gravity = _stats.GravitationalAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0)
            {
                gravity *= _stats.JumpEndEarlyGravityModifier;
            }
            else if (_isJumping && !IsInRisingJumpArc)
            {
                var apexAntiGravBonus = Mathf.Lerp(0, _stats.ApexAntiGravityBonus, _apexRatio);
                gravity -= apexAntiGravBonus;
                //Debug.Log($"ApexAntiGravBonus: {apexAntiGravBonus}");
            }
            else if (IsInRisingJumpArc)
            {
                gravity = 0;
            }
            //Debug.Log($"Gravity: {gravity}");
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, gravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Jump

    private bool _isJumping;
    private bool _jumpPressedThisFrame;
    private bool _endedJumpEarly;
    private float _timeJumpStarted = Mathf.NegativeInfinity;
    private float _timeJumpWasPressed = Mathf.NegativeInfinity;
    private float _timeLeftGround;

    private float _airJumpCharges;

    private bool HasInputJump => _jumpPressedThisFrame || HasBufferedJump;
    private bool CanGroundJump => _isGrounded || CanUseCoyote;
    private bool CanWallJump => _abilities.WallJumpLearnt && IsTouchingWall;
    private bool CanAirJump => !_isGrounded && !_isOnWall && _airJumpCharges > 0;

    private bool IsInRisingJumpArc => !_endedJumpEarly && _timeJumpStarted + _stats.JumpAccelerationDuration > _time;
    private bool HasBufferedJump => !_isJumping && _timeJumpWasPressed + _stats.JumpBuffer >= _time;
    private bool CanUseCoyote => !_isGrounded && !_isOnWall && !_isJumping && _timeLeftGround + _stats.CoyoteTime > _time;
    private bool IsInJumpRefractoryPeriod => _timeJumpStarted + _stats.JumpRefractoryPeriod > _time;

    private void HandleJump()
    {
        var jumpExecuted = false;
        if (HasInputJump && !IsInJumpRefractoryPeriod)
        {
            if (CanGroundJump) { ExecuteGroundJump(); jumpExecuted = true; }
            else if (CanWallJump) { ExecuteWallJump(); jumpExecuted = true; }
            else if (CanAirJump) { ExecuteAirJump(); jumpExecuted = true; }

            if (jumpExecuted)
            {
                _isJumping = true;
                _timeJumpStarted = _time;
                _timeJumpWasPressed = Mathf.NegativeInfinity; // Clear buffer
                _endedJumpEarly = false; // Unset this here in case we're jumping again after cutting short a previous jump (e.g., double jump)
            }
        }
        if (!jumpExecuted && IsInRisingJumpArc) // We don't want additional acceleration on the frame the jump was executed
        {
            _frameVelocity.y += _stats.JumpAcceleration * Time.fixedDeltaTime;
        }
    }

    private void ExecuteGroundJump()
    {
        _frameVelocity.y = _stats.JumpPower;
    }

    private void ExecuteWallJump()
    {
        float direction = 0;
        // Prioritize jumping left-to-right
        if (BodyContacts.IsTouchingWallLeft) direction = -1;
        else if (BodyContacts.IsTouchingWallRight) direction = 1;
        if (direction == 0) return;

        var jumpPower = _abilities.WallJumpPower > 0 ? _abilities.WallJumpPower : _stats.JumpPower;
        var jumpAngle = _abilities.WallJumpAngle * direction;
        var jumpVector = Quaternion.Euler(0, 0, jumpAngle) * Vector2.up;
        _frameVelocity = jumpPower * jumpVector;
    }

    private void ExecuteAirJump()
    {
        _airJumpCharges--;
        _frameVelocity.y = _abilities.AirJumpPower > 0 ? _abilities.AirJumpPower : _stats.JumpPower;
    }

    #endregion

    private void ApplyMovement()
    {
        _movementStrategy.Move(_frameVelocity);
    }

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning($"Please assign a {nameof(PlayerStats)} asset to the Player Controller's Stats slot", this);
        if (_abilities == null) Debug.LogWarning($"Please assign a {nameof(PlayerAbilities)} asset to the Player Controller's Abilties slot", this);
    }
#endif
}
