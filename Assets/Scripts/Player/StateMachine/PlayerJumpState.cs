using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log($"Entering PlayerState [ Jump ]");
    }

    public override void ExitState()
    {
        Debug.Log($"Exiting PlayerState [ Jump ]");
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
