using UnityEngine;

public class PlayerAirDashState : PlayerDashState
{
    public PlayerAirDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Air Dash ]");

        --player.AirDashCharges;

        // TODO: Work out how to freeze on frame 2.
        player.Animator.Play(PlayerController.GroundDashAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Air Dash ]");
    }

    public override void CheckForTransition()
    {
        // PRIO: _WallJump_ > _WallSlide_ > ...
        base.CheckForTransition();
        if (stateMachine.Transitioning) { }
        if (player.BodyContacts.Wall)
        {
            if (player.HasValidJumpInput && player.Abilities.WallJumpLearnt)
            {
                stateMachine.ChangeState(player.WallJumpState);
                return;
            }
            if (player.IsMovingAgainstWall && player.Abilities.WallSlideLearnt)
            {
                stateMachine.ChangeState(player.WallSlideState);
                return;
            }
        }
    }
}
