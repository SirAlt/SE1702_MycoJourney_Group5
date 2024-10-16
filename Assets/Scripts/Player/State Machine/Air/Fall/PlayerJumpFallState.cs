using UnityEngine;

public class PlayerJumpFallState : PlayerFallState, IJumpState
{
    #region Apex Modifier

    private IJumpState _apexModifier;
    private IJumpState ApexModifier => _apexModifier ?? new DefaultApexModifier(player, stateMachine);

    public float ApexRatio => ApexModifier.ApexRatio;

    public void ApplyApexModifier() => ApexModifier.ApplyApexModifier();

    private class DefaultApexModifier : PlayerJumpState
    {
        public DefaultApexModifier(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
        {
        }
    }

    #endregion

    public PlayerJumpFallState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if (stateMachine.PrevState is not PlayerJumpState prevApexModifierState)
        {
            //Debug.LogWarning($"Suspicious transition. Previous state was not a [ Jump ] state.");
        }
        else
        {
            _apexModifier = prevApexModifierState;
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
