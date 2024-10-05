using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool SlashPressedThisFrame { get; private set; }
    public bool DashPressed { get; private set; }

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
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpPressedThisFrame = _jumpAction.WasPressedThisFrame();
        JumpHeld = _jumpAction.IsPressed();
        SlashPressedThisFrame = _slashAction.WasPressedThisFrame();
        DashPressed = _dashAction.WasPressedThisFrame();
    }
}
