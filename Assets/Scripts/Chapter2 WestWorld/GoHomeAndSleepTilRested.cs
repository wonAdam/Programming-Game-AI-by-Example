using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeAndSleepTilRested : State<Miner>
{
    Miner _miner;
    public GoHomeAndSleepTilRested(Miner miner)
    {
        _miner = miner;
    }
    public override void Enter()
    {
        _miner.transform.position = _miner.HomePos;
        _miner.SetTextBox("집으로 걸어간다.");
    }

    public override void Execute()
    {
        base.Execute();

        _miner.SetTextBox("쿨쿨...");

        _miner.fatigue = Mathf.Max(_miner.fatigue - Time.deltaTime, 0f);

        if (_miner.RestWell() && DelayedEnough())
            _miner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_miner));
    }

    public override void Exit()
    {
        _miner.SetTextBox("정말 환상적인 낮잠이었구나! 금을 더 캐야 할 시간이다.");
    }
}
