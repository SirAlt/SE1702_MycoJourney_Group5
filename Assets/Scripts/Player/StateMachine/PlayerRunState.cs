using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log($"Entering PlayerState [ Run ]");
    }

    public override void ExitState()
    {
        Debug.Log($"Exiting PlayerState [ Run ]");
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
