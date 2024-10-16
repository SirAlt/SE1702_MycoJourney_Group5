public class PlayerAirDashState : PlayerDashState, IAirState
{
    public PlayerAirDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        --player.AirDashCharges;

        player.Animator.Play(PlayerController.AirDashStartAnim, -1, 0f);
    }

    public override void CheckForTransition()
    {
        // PRIO: _WallJump_ > _WallSlide_ > ...

        if (player.BodyContacts.Wall)
        {
            if (player.Stats.CanJumpCancelDash && player.Abilities.WallJumpLearnt && player.HasValidJumpInput)
            {
                stateMachine.ChangeState(player.WallJumpState);
                return;
            }
            if (player.Abilities.WallSlideLearnt && player.IsMovingAgainstWall)
            {
                stateMachine.ChangeState(player.WallSlideState);
                return;
            }
        }

        base.CheckForTransition();
    }


    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
        // Flinch -> AirFlinchState
    }

    public void HandleAirControl()
    {
        // Do nothing. Character cannot change direction while dashing.
    }

    public void HandleGravity()
    {
        // Do nothing. Gravity is ignored during a dash.
    }
}
