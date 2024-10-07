using UnityEngine;

public class PlayerWallJumpState : PlayerJumpState
{
    protected Vector2 jumpVector;

    public PlayerWallJumpState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Wall Jump ]");

        float direction = 0;
        // Prioritize jumping left-to-right
        if (player.BodyContacts.WallLeft) { direction = -1; player.FaceRight(); }
        else if (player.BodyContacts.WallRight) { direction = 1; player.FaceLeft(); }
        if (direction == 0)
        {
            Debug.LogError("Illegal transition. Character is not in contact with any walls.");
            stateMachine.Reset();
            return;
        }

        var jumpPower = player.Abilities.WallJumpPower > 0 ? player.Abilities.WallJumpPower : player.Stats.JumpPower;
        var jumpAngle = player.Abilities.WallJumpAngle * direction;
        var jumpDirection = Quaternion.Euler(0, 0, jumpAngle) * Vector2.up;
        jumpVector = jumpPower * jumpDirection;
        player.FrameVelocity = jumpVector;

        player.Animator.Play(PlayerController.WallJumpAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Wall Jump ]");
    }

    public override void PhysicsUpdate()
    {
        if (!jumpEndedEarly) CheckIfJumpEndedEarly();

        var pastKickAwayPeriod = jumpEndedEarly || player.BodyContacts.Ceiling || player.TimeJumpStarted + player.Stats.WallJumpKickawayPeriod <= Time.time;
        if (pastKickAwayPeriod)
        {
            base.PhysicsUpdate();
        }
        else
        {
            UpdateFacing();
            CheckCeiling();
        }
    }

    protected override void CheckIfJumpEndedEarly()
    {
        if (!player.Input.JumpHeld
            && player.TimeJumpStarted + player.Stats.WallJumpLockinTime <= Time.time
            && player.TimeJumpStarted + player.Stats.JumpEndEarlyWindow > Time.time)
        {
            jumpEndedEarly = true;
        }
    }
}
