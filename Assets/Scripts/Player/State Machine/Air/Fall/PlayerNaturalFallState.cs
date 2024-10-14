using UnityEngine;

public class PlayerNaturalFallState : PlayerFallState
{
    public PlayerNaturalFallState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.Animator.Play(PlayerController.NaturalFallAnim, -1, 0f);
    }

    public override void CheckForTransition()
    {
        // PRIO: ... > _(Coyote)GroundJump_ > AirJump
        base.CheckForTransition();
        if (stateMachine.Transitioning && stateMachine.NextState is not (PlayerAirJumpState or PlayerAirSlashState)) return;
        if (player.HasValidJumpInput && player.TimeLeftGround + player.Stats.CoyoteTime > Time.time)
        {
            stateMachine.ChangeState(player.GroundJumpState);
            return;
        }
    }
}
