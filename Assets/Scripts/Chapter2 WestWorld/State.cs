using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    protected float delay = 0f;
    protected Coroutine textShowCoroutine;
    public abstract void Enter();
    public virtual void Execute() { delay += Time.deltaTime; }
    public abstract void Exit();
    public virtual bool DelayedEnough() => delay >= 5f;
    
}
