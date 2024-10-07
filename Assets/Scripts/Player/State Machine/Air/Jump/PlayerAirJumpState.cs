using UnityEngine;

public class PlayerAirJumpState : PlayerJumpState
{
    public PlayerAirJumpState(PlayerController host, PlayerStateMachine stateMachine) : base(host, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"Entering PlayerState [ Air Jump ]");
        
        --player.AirJumpCharges;
        player.FrameVelocity.y = player.Abilities.AirJumpPower > 0 ? player.Abilities.AirJumpPower : player.Stats.JumpPower;

        player.Animator.Play(PlayerController.AirJumpAnim, -1, 0f);
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"Exiting PlayerState [ Air Jump ]");
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        base.OnAnimationEventTriggered(triggerType);
    }
}
