public abstract class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void CheckForTransition()
    {
        // PRIO: ... > _WallSlide_ > AirJump
        base.CheckForTransition();
        if (stateMachine.Transitioning && stateMachine.CurrentState is not PlayerAirJumpState) return;
        if (player.IsMovingAgainstWall && player.Abilities.WallSlideLearnt)
        {
            stateMachine.ChangeState(player.WallSlideState);
            return;
        }
    }
}
