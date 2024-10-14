using UnityEngine;

public abstract class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        if (stateMachine.PrevState is PlayerGroundState) return;
        player.AirJumpCharges = player.Abilities.AirJumpCharges;
        player.AirDashCharges = player.Abilities.AirDashCharges;
    }

    public override void ExitState()
    {
        base.ExitState();
        if (stateMachine.NextState is PlayerGroundState) return;
        player.FrameVelocity.y = 0; // Cancel grounding force.
        player.TimeLeftGround = Time.time;
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        if (stateMachine.Transitioning) return;
        if (!player.BodyContacts.Ground)
        {
            stateMachine.ChangeState(player.NaturalFallState);
            return;
        }
        if (player.HasValidDashInput)
        {
            stateMachine.ChangeState(player.GroundDashState);
            return;
        }
        if (player.HasValidJumpInput)
        {
            stateMachine.ChangeState(player.GroundJumpState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.FrameVelocity.y = -1.0f * player.Stats.GroundingVelocity;
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
        // Flinch -> GroundFlinchState
    }
}
