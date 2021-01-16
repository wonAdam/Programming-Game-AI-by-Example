using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeAndSleepTilRested : State<Miner>
{
    public GoHomeAndSleepTilRested(Miner miner) : base(miner) { }


    public override void Enter(State<Miner> previousState)
    {
        _owner.transform.position = _owner.HomePos;
        _owner.SetTextBox("집으로 걸어간다.");
    }

    public override void Execute()
    {
        base.Execute();

        _owner.SetTextBox("쿨쿨...");

        _owner.fatigue = Mathf.Max(_owner.fatigue - Time.deltaTime, 0f);

        if (_owner.RestWell() && DelayedEnough())
            _owner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_owner));
    }

    public override void Exit(State<Miner> nextState)
    {
        _owner.SetTextBox("정말 환상적인 낮잠이었구나! 금을 더 캐야 할 시간이다.");
    }
}
