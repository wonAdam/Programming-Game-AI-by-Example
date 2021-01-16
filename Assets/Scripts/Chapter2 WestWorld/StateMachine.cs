using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    T _owner;
    public State<T> currentState = null;
    public State<T> previousState = null;
    public State<T> globalState = null;

    public StateMachine(T owner, State<T> initialState)
    {
        _owner = owner;
        currentState = initialState;
        initialState.Enter(null);
    }

    public void Update()
    {
        if (globalState != null) globalState.Execute();
        if (currentState != null) currentState.Execute();
    }
    public void SetCurrentState(State<T> s) => currentState = s;
    public void SetPreviousState(State<T> s) => previousState = s;
    public void SetGlobalState(State<T> s) => globalState = s;
    public void ChangeState(State<T> nextState)
    {
        previousState = currentState;
        currentState.Exit(nextState);
        currentState = nextState;
        nextState.Enter(previousState);
    }
    public void RevertToPreviousState() => ChangeState(previousState);
    public bool IsInStateOf(State<T> state) => currentState.GetType() == state.GetType();
    

}
