using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Householding : State<MinersWife>
{
    Coroutine shouldpeepee;
    public Householding(MinersWife wife) : base(wife) { }

    public override void Enter(State<MinersWife> previousState)
    {
        _owner.transform.position = _owner.HousePos;

        _owner.StartPeePeeCoroutine(ref shouldpeepee);
    }
    public override void Execute()
    {
        base.Execute();

        if (_owner.shouldGoToToilet && DelayedEnough())
            _owner.StateMachine.ChangeState(new VisitToilet(_owner));

        _owner.SetTextBox("바닥을 닦는다.");
    }

    public override void Exit(State<MinersWife> nextState)
    {
        _owner.StopPeePeeCoroutine(ref shouldpeepee);

        if (nextState.GetType() == typeof(VisitToilet)) 
            _owner.SetTextBox("화장실을 가야겠어.");
        else if (nextState.GetType() == typeof(MakingStew))
            _owner.SetTextBox("");
    }

    


}
