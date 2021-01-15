using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMineAndDigForNugget : State<Miner>
{
    Miner _miner;
    public EnterMineAndDigForNugget(Miner miner)
    {
        _miner = miner;
    }
    public override void Enter()
    {
        _miner.transform.position = _miner.MinePos;
        _miner.SetTextBox("금광으로 걸어간다.");

    }

    public override void Execute()
    {
        base.Execute();

        _miner.goldCarried += Time.deltaTime;
        _miner.fatigue += Time.deltaTime;

        _miner.SetTextBox("금덩어리를 집는다.");

        if (_miner.PocketFull() && DelayedEnough())
            _miner.StateMachine.ChangeState(new VisitBankAndDepositGold(_miner));

        if(_miner.Thirsty() && DelayedEnough())
            _miner.StateMachine.ChangeState(new QuenchThirst(_miner));

    }

    public override void Exit()
    {

    }
}
