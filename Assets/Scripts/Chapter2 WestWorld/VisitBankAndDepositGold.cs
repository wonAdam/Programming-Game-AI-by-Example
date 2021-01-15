using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitBankAndDepositGold : State<Miner>
{
    Miner _miner;
    public VisitBankAndDepositGold(Miner miner)
    {
        _miner = miner;
    }
    public override void Enter()
    {
        _miner.transform.position = _miner.BankPos;
        _miner.SetTextBox("은행으로 간다.");
    }

    public override void Execute()
    {
        base.Execute();

        _miner.moneyInBank += _miner.goldCarried;
        _miner.goldCarried = 0f;

        _miner.SetTextBox($"금을 맡긴다. 총 저축량은 {(int)_miner.moneyInBank}이다.");

        if (_miner.AmIRichEnough() && DelayedEnough())
            _miner.StateMachine.ChangeState(new GoHomeAndSleepTilRested(_miner));

        if (!_miner.AmIRichEnough() && DelayedEnough())
            _miner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_miner));
    }

    public override void Exit()
    {
    }
}
