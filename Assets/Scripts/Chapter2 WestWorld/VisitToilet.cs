using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitToilet : State<MinersWife>
{
    public VisitToilet(MinersWife wife) : base(wife) { }
    public override void Enter(State<MinersWife> previousState)
    {
        _owner.transform.position = _owner.ToiletPos;

        _owner.SetTextBox("통으로 걸어간다. 깨끗이 소변을 봐야겠구나.");

    }
    public override void Execute()
    {
        base.Execute();

        if (DelayedEnough())
            _owner.StateMachine.ChangeState(_owner.StateMachine.previousState);

        _owner.SetTextBox("아! 시원하구나!");

    }


    public override void Exit(State<MinersWife> nextState)
    {
        _owner.SetTextBox("화장실에서 나온다.");

    }

}
