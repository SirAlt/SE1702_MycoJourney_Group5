using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Run ]");

        var offset = 0f;
        if (stateMachine.PreviousState is not PlayerGroundState) offset = 0.25f;
        player.Animator.Play(PlayerController.RunAnim, -1, offset);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Run ]");
    }

    public override void CheckForTransition()
    {
        base.CheckForTransition();
        if (stateMachine.Transitioning) return;
        if (player.Input.Move.x == 0)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.FrameVelocity.x = Mathf.MoveTowards(player.FrameVelocity.x,
                                           player.Stats.MaxGroundSpeed * Mathf.Sign(player.Input.Move.x),
                                           player.Stats.GroundAcceleration * Time.fixedDeltaTime);
    }
}
