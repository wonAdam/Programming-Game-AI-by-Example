using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    protected T _owner;
    protected float delay = 0f;
    protected Coroutine textShowCoroutine;
    public State(T owner)
    {
        _owner = owner;
    }
    public abstract void Enter(State<T> previousState);
    public virtual void Execute() { delay += Time.deltaTime; }
    public abstract void Exit(State<T> nextState);
    public virtual bool DelayedEnough() => delay >= 5f;
    
}
