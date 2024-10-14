using UnityEngine;

public class PlayerDashSlashState : PlayerDashState, IAttackState
{
    public PlayerDashSlashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        player.Input.ClearSlash();

        RemoveInvincibility();

        player.FrameVelocity.x = player.Stats.DashSlashMoveSpeed * Mathf.Sign(player.FrameVelocity.x);

        player.Animator.Play(PlayerController.DashSlashAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        DeactivateAttack();
    }

    public override void CheckForTransition()
    {
        // PRIO: *WallJump* > *GroundJump* > *AirJump* > Run > Idle > NatFall
        if (player.Stats.CanJumpCancelDash && player.HasValidJumpInput)
        {
            if (player.BodyContacts.Ground || player.TimeLeftGround + player.Stats.CoyoteTime > Time.time)
            {
                stateMachine.ChangeState(player.GroundJumpState);
                return;
            }
            if (player.Abilities.WallJumpLearnt && player.BodyContacts.Wall)
            {
                stateMachine.ChangeState(player.WallJumpState);
                return;
            }
            if (player.AirDashCharges > 0)
            {
                stateMachine.ChangeState(player.AirJumpState);
                return;
            }
        }

        // Run > Idle > NatFall are transition on attack finished.
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        switch (triggerType)
        {
            case PlayerController.AnimationTriggerType.AttackActiveFramesStarted:
                ActivateAttack();
                goto default;
            case PlayerController.AnimationTriggerType.AttackActiveFramesEnded:
                DeactivateAttack();
                goto default;
            case PlayerController.AnimationTriggerType.AttackFinished:
                if (player.BodyContacts.Ground)
                    player.LandImmediate();
                else
                    stateMachine.ChangeStateImmediate(player.NaturalFallState);
                goto default;
            default:
                base.OnAnimationEventTriggered(triggerType);
                break;
        }
    }

    public void ActivateAttack()
    {
        player.DashSlash.Activate();
        player.TimeSlashActivated = Time.time;
    }

    public void DeactivateAttack() => player.DashSlash.Deactivate();
}
