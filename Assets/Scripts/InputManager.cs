using UnityEngine;
using UnityEngine.InputSystem;

// We need this to execute late so its FixedUpdate can clear the recorded inputs
// _after_ other scripts have had the chance to use them.
[DefaultExecutionOrder(1337)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Vector2 Move { get; private set; }

    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpHeld { get; private set; }
    public float TimeJumpWasPressed { get; private set; } = Mathf.NegativeInfinity;

    public bool SlashPressedThisFrame { get; private set; }

    public bool DashPressedThisFrame { get; private set; }
    public bool DashHeld { get; private set; }
    public float TimeDashWasPressed { get; private set; } = Mathf.NegativeInfinity;

    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _slashAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _dashAction = _playerInput.actions["Dash"];
        _slashAction = _playerInput.actions["Slash"];
    }

    private void OnEnable()
    {
        _moveAction?.Enable();
        _jumpAction?.Enable();
        _dashAction?.Enable();
        _slashAction?.Enable();
    }

    private void OnDisable()
    {
        _moveAction?.Disable();
        _jumpAction?.Disable();
        _dashAction?.Disable();
        _slashAction?.Disable();
    }

    private void Update()
    {
        Move = _moveAction.ReadValue<Vector2>();

        if (_jumpAction.WasPerformedThisFrame())
        {
            JumpPressedThisFrame = true;
            TimeJumpWasPressed = Time.time;
        }
        JumpHeld = _jumpAction.IsPressed();

        SlashPressedThisFrame = _slashAction.WasPressedThisFrame();

        if (_dashAction.WasPerformedThisFrame())
        {
            DashPressedThisFrame = true;
            TimeDashWasPressed = Time.time;
        }
        DashHeld = _dashAction.IsPressed();
    }

    private void FixedUpdate()
    {
        JumpPressedThisFrame = false;
        DashPressedThisFrame = false;
    }

    public void ClearJump()
    {
        JumpPressedThisFrame = false;
        TimeJumpWasPressed = Mathf.NegativeInfinity;
    }

    public void ClearDash()
    {
        DashPressedThisFrame = false;
        TimeDashWasPressed = Mathf.NegativeInfinity;
    }
}
