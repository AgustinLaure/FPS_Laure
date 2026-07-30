using System;
using System.Collections.Generic;

public class FSM
{
    private IState currentState;

    private Dictionary<Type, IState> statesDictionary = new Dictionary<Type, IState>();

    public FSM(Dictionary<Type, IState> states)
    {
        statesDictionary = states;
    }

    public void Update()
    {
        currentState.Update();
    }

    public void SetInitialState(Type initialState)
    {
      statesDictionary.TryGetValue(initialState, out currentState);
        currentState?.Enter();
    }
    public void TryChange<T>(Type toState) where T : IState
    {
        if (currentState is T)
        {
            currentState?.Exit();
            statesDictionary.TryGetValue(toState, out currentState);
            currentState?.Enter();
        }
    }
}
