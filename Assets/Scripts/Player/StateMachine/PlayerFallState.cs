using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log($"Entering PlayerState [ Fall ]");
    }

    public override void ExitState()
    {
        Debug.Log($"Exiting PlayerState [ Fall ]");
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
