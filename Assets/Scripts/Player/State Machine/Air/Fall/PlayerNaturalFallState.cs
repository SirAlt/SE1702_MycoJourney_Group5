using UnityEngine;

public class PlayerNaturalFallState : PlayerFallState
{
    public PlayerNaturalFallState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Natural Fall ]");
        player.Animator.Play(PlayerController.NaturalFallAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Natural Fall ]");
    }

    public override void CheckForTransition()
    {
        // PRIO: ... > _Ground[Coyote]Jump_ > AirJump
        base.CheckForTransition();
        if (stateMachine.Transitioning && stateMachine.CurrentState is not PlayerAirJumpState) return;
        if (player.HasValidJumpInput && player.TimeLeftGround + player.Stats.CoyoteTime > Time.time)
        {
            stateMachine.ChangeState(player.GroundJumpState);
            return;
        }
    }
}
