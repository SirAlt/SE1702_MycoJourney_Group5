using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    protected float gravity;

    public PlayerAirState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
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
        if (player.HasValidJumpInput)
        {
            if (player.BodyContacts.Wall && player.Abilities.WallJumpLearnt)
            {
                stateMachine.ChangeState(player.WallJumpState);
                return;
            }
            else if (player.AirJumpCharges > 0)
            {
                stateMachine.ChangeState(player.AirJumpState);
                return;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        HandleAirControl();
        HandleGravity();
    }

    protected virtual void HandleAirControl()
    {
        if (player.Input.Move.x == 0)
        {
            player.FrameVelocity.x = Mathf.MoveTowards(player.FrameVelocity.x, 0, player.Stats.AirDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            player.FrameVelocity.x = Mathf.MoveTowards(player.FrameVelocity.x,
                                                       player.Stats.MaxAirSpeed * Mathf.Sign(player.Input.Move.x),
                                                       player.Stats.GroundAcceleration * Time.fixedDeltaTime);
        }
    }

    protected virtual void HandleGravity()
    {
        gravity = player.Stats.GravitationalAcceleration;
        player.FrameVelocity.y = Mathf.MoveTowards(player.FrameVelocity.y, -1.0f * player.Stats.MaxFallSpeed, gravity * Time.fixedDeltaTime);
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
        // Flinch -> AirFlinchState
    }
}
