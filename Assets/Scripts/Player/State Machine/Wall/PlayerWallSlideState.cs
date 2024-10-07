using UnityEngine;

public class PlayerWallSlideState : PlayerWallState
{
    public PlayerWallSlideState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Wall Slide ]");
        player.Animator.Play(PlayerController.WallSlideAnim, -1, 0f);
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        Debug.Log($"Exiting PlayerState [ Wall Slide ]");
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
