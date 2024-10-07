using UnityEngine;
using static PlayerJumpState;

public class PlayerJumpFallState : PlayerFallState, IApexModifier
{
    #region Apex Modifier

    private PlayerJumpState _preceedingJumpState;

    public float ApexRatio => _preceedingJumpState.ApexRatio;

    public void ApplyApexModifier() => _preceedingJumpState.ApplyApexModifier();

    #endregion

    public PlayerJumpFallState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Jump Fall ]");
        
        if (stateMachine.PreviousState is not PlayerJumpState preceedingJumpState)
        {
            Debug.LogError($"Illegal transition. Previous state was not a [ Jump ] state.");
            stateMachine.Reset();
            return;
        }
        _preceedingJumpState = preceedingJumpState;

        player.Animator.Play(PlayerController.JumpFallAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Jump Fall ]");
    }

    protected override void HandleGravity()
    {
        gravity = player.Stats.GravitationalAcceleration;
        ApplyApexModifier();
        player.FrameVelocity.y = Mathf.MoveTowards(player.FrameVelocity.y, -1.0f * player.Stats.MaxFallSpeed, gravity * Time.fixedDeltaTime);
    }
}
