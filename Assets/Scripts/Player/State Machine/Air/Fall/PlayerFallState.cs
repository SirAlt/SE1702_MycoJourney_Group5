public abstract class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void CheckForTransition()
    {
        // PRIO: ... > _WallSlide_ > AirJump
        base.CheckForTransition();
        if (stateMachine.Transitioning && stateMachine.NextState is not (PlayerAirJumpState or PlayerAirSlashState)) return;
        if (player.Abilities.WallSlideLearnt && player.IsMovingAgainstWall)
        {
            stateMachine.ChangeState(player.WallSlideState);
            return;
        }
    }
}
