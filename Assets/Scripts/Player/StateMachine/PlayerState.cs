using static PlayerController;

public abstract class PlayerState
{
    protected PlayerController _host;
    protected PlayerStateMachine _stateMachine;

    public PlayerState(PlayerController host, PlayerStateMachine stateMachine)
    {
        _host = host;
        _stateMachine = stateMachine;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void FrameUpdate();
    public abstract void PhysicsUpdate();
    public abstract void TriggerAnimation(AnimationTriggerType triggerType);
}
