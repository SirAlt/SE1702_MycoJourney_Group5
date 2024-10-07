using System;
using UnityEngine;

public class PlayerStateMachine
{
    public bool Transitioning { get; private set; }
    public PlayerState PreviousState { get; private set; }
    public PlayerState CurrentState { get; private set; }

    private PlayerState _startingState;

    public void Initialize(PlayerState startingState)
    {
        _startingState = startingState;
        PreviousState = null;
        CurrentState = startingState;
        CurrentState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        Transitioning = true;
        PreviousState = CurrentState;
        CurrentState = newState;
    }

    public void ChangeStateImmediate(PlayerState newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;
        PreviousState.ExitState();
        CurrentState.EnterState();
    }

    public void FrameUpdate()
    {
        //CheckForTransition();
        CurrentState.FrameUpdate();
    }

    public void PhysicsUpdate()
    {
        CheckForTransition(chain: false);
        CurrentState.PhysicsUpdate();
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
            Initialize(_startingState);
        }
    }

    //          ### WARNING ###
    // DO NOT USE chain mode. It has many
    // unfixed bugs, including infinite loops.
    private void CheckForTransition(bool chain)
    {
        do
        {
            Transitioning = false;
            CurrentState.CheckForTransition();
        } while (chain && Transitioning);

        if (Transitioning)
        {
            Transitioning = false;
            PreviousState.ExitState();
            CurrentState.EnterState();
        }
    }
}
