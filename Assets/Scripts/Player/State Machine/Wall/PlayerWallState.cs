public abstract class PlayerWallState : PlayerState
{
    public PlayerWallState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        if (player.Abilities.RechargeJumpsOnWall)
            player.AirJumpCharges = player.Abilities.AirJumpCharges;
        if (player.Abilities.RechargeDashesOnWall)
            player.AirDashCharges = player.Abilities.AirDashCharges;
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        if (stateMachine.Transitioning) return;
        if (player.BodyContacts.Ground)
        {
            player.Land();
            return;
        }
        if (player.HasValidDashInput && player.AirDashCharges > 0)
        {
            stateMachine.ChangeState(player.AirDashState);
            return;
        }
        if (!player.BodyContacts.Wall)
        {
            stateMachine.ChangeState(player.NaturalFallState);
            return;
        }
        if (player.HasValidJumpInput && player.Abilities.WallJumpLearnt)
        {
            stateMachine.ChangeState(player.WallJumpState);
            return;
        }
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
        // if Flinch -> AirFlinchState
    }
}
