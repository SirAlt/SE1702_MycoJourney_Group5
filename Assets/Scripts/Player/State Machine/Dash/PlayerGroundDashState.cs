using UnityEngine;

public class PlayerGroundDashState : PlayerDashState
{
    public PlayerGroundDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Ground Dash ]");

        // TODO: Work out how to freeze on frame 2.
        player.Animator.Play(PlayerController.GroundDashAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Ground Dash ]");
    }
}
