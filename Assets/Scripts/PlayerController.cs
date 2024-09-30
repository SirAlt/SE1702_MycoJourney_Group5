using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerAbilities _abilities;

    #region Input

    private struct Input
    {
        public Vector2 move;
        public bool jumpPressed;
        public bool dashPressed;
        public bool slashPressed;
        public bool shootPressed;
    }

    private Input _input;

    #region InputAction callbacks

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _slashAction;
    private InputAction _shootAction;

    public void OnMovePerformed(CallbackContext context)
    {
        _input.move = context.ReadValue<Vector2>();
    }

    public void OnMoveCancelled(CallbackContext context)
    {
        _input.move = Vector2.zero;
    }

    public void OnJumpPerformed(CallbackContext context)
    {
        _input.jumpPressed = true;
        _jumpPressedThisFrame = true;
        _timeJumpWasPressed = _time;
    }

    public void OnJumpCancelled(CallbackContext context)
    {
        _input.jumpPressed = false;
    }

    public void OnDashPerformed(CallbackContext context)
    {
        _input.dashPressed = true;
    }

    public void OnDashCancelled(CallbackContext context)
    {
        _input.dashPressed = false;
    }

    public void OnSlashPerformed(CallbackContext context)
    {
        _input.slashPressed = true;
    }

    public void OnSlashCancelled(CallbackContext context)
    {
        _input.slashPressed = false;
    }

    public void OnShootPerformed(CallbackContext context)
    {
        _input.shootPressed = true;
    }

    public void OnShootCancelled(CallbackContext context)
    {
        _input.shootPressed = false;
    }

    #endregion

    private void OnEnable()
    {
        InputActionAsset inputActionAsset = GetComponent<PlayerInput>().actions;

        _moveAction = inputActionAsset.FindAction("Move");
        _jumpAction = inputActionAsset.FindAction("Jump");
        _dashAction = inputActionAsset.FindAction("Dash");
        _slashAction = inputActionAsset.FindAction("Slash");
        _shootAction = inputActionAsset.FindAction("Shoot");

        _moveAction.Enable();
        _jumpAction.Enable();
        _dashAction.Enable();
        _slashAction.Enable();
        _shootAction.Enable();

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCancelled;
        _jumpAction.performed += OnJumpPerformed;
        _jumpAction.canceled += OnJumpCancelled;
        _dashAction.performed += OnDashPerformed;
        _dashAction.canceled += OnDashCancelled;
        _slashAction.performed += OnSlashPerformed;
        _slashAction.canceled += OnSlashCancelled;
        _shootAction.performed += OnShootPerformed;
        _shootAction.canceled += OnShootCancelled;
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
        _dashAction.Disable();
        _slashAction.Disable();
        _shootAction.Disable();

        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCancelled;
        _jumpAction.performed -= OnJumpPerformed;
        _jumpAction.canceled -= OnJumpCancelled;
        _dashAction.performed -= OnDashPerformed;
        _dashAction.canceled -= OnDashCancelled;
        _slashAction.performed -= OnSlashPerformed;
        _slashAction.canceled -= OnSlashCancelled;
        _shootAction.performed -= OnShootPerformed;
        _shootAction.canceled -= OnShootCancelled;
    }

    #endregion

    private Rigidbody2D _rb;
    private Collider2D _collider;

    private float _time;
    private bool _queryStartInCollidersCached;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _queryStartInCollidersCached = Physics2D.queriesStartInColliders;
        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_stats.TerrainLayers);
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }

    #region Physics

    private Vector2 _frameVelocity;

    private ContactFilter2D _contactFilter;
    private readonly List<RaycastHit2D> _raycastHits = new();

    private void FixedUpdate()
    {
        //Debug.Log($"FixedUpdate: Time={_time}");

        //Debug.Log($"Position: {_rb.position}");
        Physics2D.queriesStartInColliders = false;

        CheckApex();

        CheckWall();
        HandleMovement();

        CheckGround();
        CheckCeiling();
        HandleAirControl();
        HandleGravity();
        HandleJump();

        StepX();
        StepY();

        Physics2D.queriesStartInColliders = _queryStartInCollidersCached;
        //Debug.Log($"Frame velocity: {_frameVelocity}");
    }

    #region Apex Modifiers

    private float _apexRatio;

    private void CheckApex()
    {
        if (!_isJumping)
            _apexRatio = 0;
        else
            _apexRatio = Mathf.InverseLerp(_stats.ApexThreshold, 0, Mathf.Abs(_frameVelocity.y));


        //Debug.Log($"ApexRatio: {_apexRatio}");
    }

    #endregion

    #region Move X

    private bool _isTouchingWallRight;
    private bool _isTouchingWallLeft;
    private bool _isOnWall;

    private bool IsTouchingWall => _isTouchingWallRight || _isTouchingWallLeft;
    private bool IsMovingAgainstWall => (_isTouchingWallRight && _input.move.x > 0) || (_isTouchingWallLeft && _input.move.x < 0);

    private void CheckWall()
    {
        bool wallHitRight = _rb.Cast(Vector2.right, _contactFilter, _raycastHits, _stats.WallOffset) > 0;
        bool wallHitLeft = _rb.Cast(Vector2.left, _contactFilter, _raycastHits, _stats.WallOffset) > 0;

        _isTouchingWallRight = wallHitRight;
        _isTouchingWallLeft = wallHitLeft;

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
        if (_input.move.x == 0)
        {
            var deceleration = _isOnGround ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _stats.MaxHorizontalSpeed * Mathf.Sign(_input.move.x), _stats.HorizontalAcceleration * Time.fixedDeltaTime);
            if (_isJumping)
            {
                var apexVelocityBonus = _stats.ApexAccelerationBonus * _apexRatio * Time.fixedDeltaTime;
                _frameVelocity.x += apexVelocityBonus * Mathf.Sign(_input.move.x);
                //Debug.Log($"ApexVelocityBonus: {apexVelocityBonus}");
            }

            if (_input.move.x < 0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            else transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    #endregion

    #region Move Y

    private bool _isOnGround;

    private void CheckGround()
    {
        bool groundHit = _rb.Cast(Vector2.down, _contactFilter, _raycastHits, _stats.FloorOffset + _collider.offset.y) > 0;
        //Debug.Log($"GroundCheck: {groundHit}");

        // Landed
        if (!_isOnGround && groundHit)
        {
            _isOnGround = true;
            _isOnWall = false;
            _isJumping = false;
            _endedJumpEarly = false; // If we don't unset this here, we might run into issues when the character moves upward without jumping
            _airJumpCharges = _abilities.AirJumpCharges;
            // Notify Animator
        }
        // Left ground
        else if (_isOnGround && !groundHit)
        {
            _isOnGround = false;
            _timeLeftGround = _time;
            // Notify Animator
        }
    }

    private void CheckCeiling()
    {
        bool ceilingHit = _rb.Cast(Vector2.up, _contactFilter, _raycastHits, _stats.CeilingOffset + _collider.offset.y) > 0;
        if (ceilingHit) _frameVelocity.y = Mathf.Min(_frameVelocity.y, 0);
    }

    #region Gravity

    private void HandleAirControl()
    {
        // 1. Because we have input buffering, we cannot rely on the keyup event to set this flag. The player could have already released the Jump key before even hitting the ground.
        // 2. We set this value once, and only unset it upon entering or exiting the jump state. This means a jump can be ended early exactly once, and never "un-ended".
        if (_isJumping
            && !_input.jumpPressed
            && _frameVelocity.y > 0
            && _timeJumpStarted + _stats.JumpEndEarlyWindow > _time
            && !_endedJumpEarly)
        {
            _endedJumpEarly = true;
        }
    }

    private void HandleGravity()
    {
        if (_isOnGround)
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
    private bool CanGroundJump => _isOnGround || CanUseCoyote;
    private bool CanWallJump => _abilities.WallJumpLearnt && IsTouchingWall;
    private bool CanAirJump => !_isOnGround && !_isOnWall && _airJumpCharges > 0;

    private bool IsInRisingJumpArc => !_endedJumpEarly && _timeJumpStarted + _stats.JumpAccelerationDuration > _time;
    private bool HasBufferedJump => !_isJumping && _timeJumpWasPressed + _stats.JumpBuffer >= _time;
    private bool CanUseCoyote => !_isOnGround && !_isOnWall && !_isJumping && _timeLeftGround + _stats.CoyoteTime > _time;
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
        _jumpPressedThisFrame = false;
    }

    private void ExecuteGroundJump()
    {
        _frameVelocity.y = _stats.JumpPower;
    }

    private void ExecuteWallJump()
    {
        float direction = 0;
        // Prioritize jumping left-to-right
        if (_isTouchingWallLeft) direction = -1;
        else if (_isTouchingWallRight) direction = 1;
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

    #endregion

    #region Apply movement  

    private void StepX()
    {
        var xStep = _frameVelocity.x * Time.fixedDeltaTime;
        if (xStep == 0) return;

        bool terrainHit = _rb.Cast(Vector2.right * Mathf.Sign(xStep), _contactFilter, _raycastHits, Mathf.Abs(xStep) + _stats.CollisionOffset) > 0;
        //Debug.Log($"StepX: TerrainHit={terrainHit}");

        // TODO: Slopes - TRY: RaycastHit2D.Normal

        if (terrainHit)
        {
            float distance;
            var closestDistance = Mathf.Infinity;
            foreach (var raycastHit in _raycastHits)
            {
                // TODO: Account for collider shape
                distance = xStep > 0
                   ? raycastHit.point.x - _collider.bounds.max.x
                   : _collider.bounds.min.x - raycastHit.point.x;
                if (distance < closestDistance) { closestDistance = distance; }
            }
            xStep = Mathf.Max(closestDistance - _stats.CollisionOffset, 0) * Mathf.Sign(xStep);
        }

        _rb.position = new Vector2(_rb.position.x + xStep, _rb.position.y);
    }

    private void StepY()
    {
        var yStep = _frameVelocity.y * Time.fixedDeltaTime;
        if (yStep == 0) return;

        bool terrainHit = _rb.Cast(Vector2.up * Mathf.Sign(yStep), _contactFilter, _raycastHits, Mathf.Abs(yStep) + _stats.CollisionOffset) > 0;
        //Debug.Log($"StepY: TerrainHit={terrainHit}");

        if (terrainHit)
        {
            float distance;
            var closestDistance = Mathf.Infinity;
            foreach (var raycastHit in _raycastHits)
            {
                // TODO: Account for collider shape
                distance = yStep > 0
                   ? raycastHit.point.y - _collider.bounds.max.y
                   : _collider.bounds.min.y - raycastHit.point.y;
                if (distance < closestDistance) { closestDistance = distance; }
            }
            yStep = Mathf.Max(closestDistance - _stats.CollisionOffset, 0) * Mathf.Sign(yStep);
        }

        _rb.position = new Vector2(_rb.position.x, _rb.position.y + yStep);
    }

    #endregion

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning($"Please assign a {nameof(PlayerStats)} asset to the Player Controller's Stats slot", this);
    }
#endif
}
