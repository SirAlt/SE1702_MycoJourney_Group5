using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log($"Entering PlayerState [ Idle ]");
    }

    public override void ExitState()
    {
        Debug.Log($"Exiting PlayerState [ Idle ]");
    }

    public override void FrameUpdate()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void TriggerAnimation(PlayerController.AnimationTriggerType triggerType)
    {
    }
}
