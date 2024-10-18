using UnityEngine;

public class PlayerJumpFallState : PlayerFallState, IJumpState
{
    #region Jump State

    private readonly IJumpState _defaultJumpState;

    private IJumpState _jumpState;

    public float ApexRatio => _jumpState.ApexRatio;

    public void ApplyApexModifier() => _jumpState.ApplyApexModifier();

    private class DefaultJumpState : PlayerJumpState
    {
        public DefaultJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
        {
        }
    }

    #endregion

    public PlayerJumpFallState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        _defaultJumpState = new DefaultJumpState(player, stateMachine);
    }

    public override void EnterState()
    {
        base.EnterState();

        if (stateMachine.PrevState is not PlayerJumpState prevJumpState)
        {
            //Debug.LogWarning($"Previous state was not a [ Jump ] state. Using defaults for jump state logics.");
            _jumpState = _defaultJumpState;
        }
        else
        {
            _jumpState = prevJumpState;
        }

        if (stateMachine.PrevState is not PlayerFallState)
            player.Animator.Play(PlayerController.JumpFallAnim, -1, 0f);
    }

    // TRACE: This state implements custom physics.
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void HandleGravity()
    {
        gravity = player.Stats.GravitationalAcceleration;
        ApplyApexModifier();
        player.FrameVelocity.y = Mathf.MoveTowards(player.FrameVelocity.y, -1.0f * player.Stats.FallSpeedClamp, gravity * Time.fixedDeltaTime);
    }

    public void ExecuteJump()
    {
        // Do nothing.
    }
}
