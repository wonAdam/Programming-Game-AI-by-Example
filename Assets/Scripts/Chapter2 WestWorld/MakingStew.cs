using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingStew : State<MinersWife>
{
    public MakingStew(MinersWife wife) : base(wife) { }

    public override void Enter(State<MinersWife> previousState)
    {
        _owner.transform.position = _owner.KitchenPos;

        _owner.SetTextBox("");

    }
    public override void Execute()
    {
        base.Execute();

        if (DelayedEnough())
            _owner.StateMachine.ChangeState(new Householding(_owner));
    }

    public override void Exit(State<MinersWife> nextState)
    {
        _owner.SetTextBox("");
    }
}
