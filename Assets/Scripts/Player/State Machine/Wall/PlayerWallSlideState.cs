using UnityEngine;

public class PlayerWallSlideState : PlayerWallState
{
    public PlayerWallSlideState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.Animator.Play(PlayerController.WallSlideAnim, -1, 0f);
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        if (!player.IsMovingAgainstWall)
        {
            stateMachine.ChangeState(player.NaturalFallState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.FrameVelocity.y = -1.0f * player.Abilities.WallSlideGravityModifier * player.Stats.GravitationalAcceleration * Time.fixedDeltaTime;
    }
}
