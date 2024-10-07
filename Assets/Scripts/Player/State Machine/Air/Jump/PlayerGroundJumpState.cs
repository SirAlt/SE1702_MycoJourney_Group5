using UnityEngine;

public class PlayerGroundJumpState : PlayerJumpState
{
    public PlayerGroundJumpState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Ground Jump ]");
        player.FrameVelocity.y = player.Stats.JumpPower;
        player.Animator.Play(PlayerController.GroundJumpAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Ground Jump ]");
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
    }
}
