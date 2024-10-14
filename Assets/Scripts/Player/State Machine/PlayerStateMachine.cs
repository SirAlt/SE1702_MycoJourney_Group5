using System;
using UnityEngine;

public class PlayerStateMachine
{
    private PlayerState _cachedStartingState;

    public bool Transitioning { get; private set; }
    public PlayerState PrevState { get; private set; }
    public PlayerState CurrentState { get; private set; }
    public PlayerState NextState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        _cachedStartingState = startingState;

        Transitioning = false;
        PrevState = null;
        CurrentState = startingState;
        NextState = null;

        CurrentState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        Transitioning = true;
        NextState = newState;
    }

    public void ChangeStateImmediate(PlayerState newState)
    {
        ChangeState(newState);
        ExecuteTransition();
    }

    public void StopTransition()
    {
        Transitioning = false;
        NextState = null;
    }

    public void FrameUpdate()
    {
        //CheckForTransition();
        CurrentState.FrameUpdate();
    }

    public void PhysicsUpdate()
    {
        CheckForTransition();
        ExecuteTransition();
        CurrentState.PhysicsUpdate();
    }

    private void CheckForTransition()
    {
        if (Transitioning) return;
        CurrentState.CheckForTransition();
    }

    private void ExecuteTransition()
    {
        if (Transitioning)
        {
            Transitioning = false;
            PrevState = CurrentState;
            CurrentState = NextState;
            NextState = null;
            PrevState.ExitState();
            CurrentState.EnterState();
            //Debug.Log($"PlayerState transition: [ {PrevState.GetType().Name[6..^5]} ] >> [ {CurrentState.GetType().Name[6..^5]} ]");
        }
    }

    public void Reset()
    {
        try
        {
            CurrentState.ExitState();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            Initialize(_cachedStartingState);
        }
    }
}
