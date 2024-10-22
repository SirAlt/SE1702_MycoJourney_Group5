﻿using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.FrameVelocity = Vector2.zero;
        player.Animator.Play(PlayerController.DeathAnim, -1, 0f);
    }

    public override void CheckForTransition()
    {
        // No. Unless... we later implement revive.
    }

    public override void PhysicsUpdate()
    {
        // No.
    }

    public override void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        if (triggerType == PlayerController.AnimationTriggerType.DeathComplete)
        {
            player.PromptRetry();
            return;
        }
    }
}