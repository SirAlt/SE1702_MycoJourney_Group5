using UnityEngine;

public class PlayerDashState : PlayerState
{
    protected Vector2 dashVector;
    protected bool dashEndedEarly;
    protected bool invincibilityGranted;

    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        player.TimeDashStarted = Time.time;
        player.Input.ClearDash();
        dashEndedEarly = false;

        ApplyDash();


        if (player.Abilities.InvincibleDash)
        {
            invincibilityGranted = true;
            // Grant invinc
        }
    }

    protected virtual void ApplyDash()
    {
        float dashDirection;
        if (player.Input.Move.x != 0)
        {
            dashDirection = Mathf.Sign(player.Input.Move.x);
        }
        else
        {
            dashDirection = player.IsFacingRight ? 1.0f : -1.0f;
        }
        dashVector = new Vector2(dashDirection * player.Stats.DashSpeed, 0);
        player.FrameVelocity = dashVector;
    }

    public override void ExitState()
    {
        base.ExitState();
        player.TimeDashEnded = Time.time;
        dashEndedEarly = false;
        if (invincibilityGranted)
        {
            invincibilityGranted = false;
            // Remove invinc
        }
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        if (stateMachine.Transitioning) return;
        if (!dashEndedEarly) CheckIfDashEndedEarly();
        if (dashEndedEarly || player.TimeDashStarted + player.Stats.DashDuration <= Time.time)
        {
            if (player.BodyContacts.Ground)
            {
                player.Land();
                return;
            }
            else
            {
                stateMachine.ChangeState(player.NaturalFallState);
                return;
            }
        }
    }

    protected virtual void CheckIfDashEndedEarly()
    {
        if (!player.Input.DashHeld
            && player.TimeDashStarted + player.Stats.DashEndEarlyWindow > Time.time)
        {
            dashEndedEarly = true;
        }
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
    }
}
