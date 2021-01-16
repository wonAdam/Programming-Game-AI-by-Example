using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitBankAndDepositGold : State<Miner>
{
    public VisitBankAndDepositGold(Miner miner) : base(miner) { }

    public override void Enter(State<Miner> previousState)
    {
        _owner.transform.position = _owner.BankPos;
        _owner.SetTextBox("은행으로 간다.");
    }

    public override void Execute()
    {
        base.Execute();

        _owner.moneyInBank += _owner.goldCarried;
        _owner.goldCarried = 0f;

        _owner.SetTextBox($"금을 맡긴다. 총 저축량은 {(int)_owner.moneyInBank}이다.");

        if (_owner.AmIRichEnough() && DelayedEnough())
            _owner.StateMachine.ChangeState(new GoHomeAndSleepTilRested(_owner));

        if (!_owner.AmIRichEnough() && DelayedEnough())
            _owner.StateMachine.ChangeState(new EnterMineAndDigForNugget(_owner));
    }

    public override void Exit(State<Miner> nextState)
    {
    }
}
