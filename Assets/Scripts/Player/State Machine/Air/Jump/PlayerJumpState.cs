using UnityEngine;
using static PlayerJumpState;

public abstract class PlayerJumpState : PlayerAirState, IApexModifier
{
    #region Apex Modifier

    public interface IApexModifier
    {
        float ApexRatio { get; }

        void ApplyApexModifier();
    }

    public virtual float ApexRatio => Mathf.InverseLerp(player.Stats.ApexThreshold, 0, Mathf.Abs(player.FrameVelocity.y));

    public virtual void ApplyApexModifier()
    {
        var apexRatio = ApexRatio;

        if (player.Input.Move.x != 0)
        {
            var apexVelocityBonus = player.Stats.ApexAccelerationBonus * apexRatio * Time.fixedDeltaTime;
            player.FrameVelocity.x += apexVelocityBonus * Mathf.Sign(player.Input.Move.x);
        }

        var apexAntiGravBonus = Mathf.Lerp(0, player.Stats.ApexAntiGravityBonus, apexRatio);
        gravity = Mathf.MoveTowards(gravity, 0, apexAntiGravBonus); // Improved logic. No longer allows "upward" gravity.
    }

    #endregion

    protected bool jumpEndedEarly;

    public PlayerJumpState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.TimeJumpStarted = Time.time;
        player.Input.ClearJump();
        jumpEndedEarly = false;
    }

    public override void ExitState()
    {
        base.ExitState();
        jumpEndedEarly = false;
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        if (stateMachine.Transitioning) return;
        if (player.FrameVelocity.y <= 0)
        {
            stateMachine.ChangeState(player.JumpFallState);
            return;
        }
    }

    // Leave this here so it's easy to trace that this state implements custom physics
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected override void HandleGravity()
    {
        gravity = player.Stats.GravitationalAcceleration;

        if (player.BodyContacts.Ceiling) goto APPLY_GRAVITY;

        if (!jumpEndedEarly) CheckIfJumpEndedEarly();

        if (jumpEndedEarly)
        {
            gravity *= player.Stats.JumpEndEarlyGravityModifier;
        }
        else if (player.TimeJumpStarted + player.Stats.InitialJumpPeriod > Time.time)
        {
            gravity = 0;
            player.FrameVelocity.y += player.Stats.InitialJumpAcceleration * Time.fixedDeltaTime;
        }
        else
        {
            ApplyApexModifier();
        }

    APPLY_GRAVITY:
        player.FrameVelocity.y = Mathf.MoveTowards(player.FrameVelocity.y, -1.0f * player.Stats.MaxFallSpeed, gravity * Time.fixedDeltaTime);
    }

    protected virtual void CheckIfJumpEndedEarly()
    {
        if (!player.Input.JumpHeld
            && player.TimeJumpStarted + player.Stats.JumpEndEarlyWindow > Time.time)
        {
            jumpEndedEarly = true;
        }
    }
}
