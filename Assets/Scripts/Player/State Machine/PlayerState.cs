using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    public PlayerState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState()
    {
    }

    public virtual void ExitState()
    {
    }

    public virtual void CheckForTransition()
    {
    }

    public virtual void FrameUpdate()
    {
    }

    public virtual void PhysicsUpdate()
    {
        UpdateFacing();
        HandleCeilingHit();
    }

    protected virtual void UpdateFacing()
    {
        if (player.Input.Move.x < 0) player.FaceLeft();
        else if (player.Input.Move.x > 0) player.FaceRight();
    }

    protected virtual void HandleCeilingHit()
    {
        if (player.BodyContacts.Ceiling)
        {
            player.FrameVelocity.y = Mathf.Min(player.FrameVelocity.y, 0);
        }
    }

    public virtual void OnAnimationEventTriggered(PlayerController.AnimationTriggerType triggerType)
    {
        if (triggerType == PlayerController.AnimationTriggerType.Flinch)
        {
            if (player.BodyContacts.Ground)
                stateMachine.ChangeState(player.GroundFlinchState);
            else
                stateMachine.ChangeState(player.AirFlinchState);
            return;
        }
    }
}
